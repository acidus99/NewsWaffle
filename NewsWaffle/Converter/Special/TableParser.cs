using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using System.Net;
using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace NewsWaffle.Converter.Special
{
    public class TableParser
    {

        Row currRow;
        Table table;
        TextExtractor textExtractor;
        //used when adding row/colspans to fix mismatched tables
        int currRowWidth;

        public TableParser()
        {
            table = new Table();
            textExtractor = new TextExtractor
            {
                ShouldConvertImages = true,
                ShouldCollapseNewlines = true
            };
        }

        public Table ParseTable(HtmlElement element)
        {
            ParseChildren(element);
            AppendRow();
            //go back and place any rowspan placeholder cells
            UpdateForRowSpans();
            return table;
        }

        private void ParseChildren(HtmlElement element)
            => element.Children.ToList().ForEach(x => ParseTag((HtmlElement) x));

        private void ParseTag(HtmlElement current)
        {

            switch(current.NodeName.ToLower())
            {
                case "caption":
                    textExtractor.Extract(current);
                    table.Caption = textExtractor.Content;
                    break;

                case "tr":
                    {
                        AppendRow();
                        currRow = new Row();
                        ParseChildren(current);
                        break;
                    }

                case "td":
                case "th":
                    AddCell(current);
                    break;

                //pass through
                case "tbody":
                case "tfoot":
                case "thead":
                    ParseChildren(current);
                    break;
            }
        }

        private void AppendRow()
        {
            if (currRow != null)
            {
                table.Rows.Add(currRow);
            }
        }

        private void AddCell(HtmlElement cell)
        {
            if (currRow != null)
            {
                textExtractor.Extract(cell);
                string contents = textExtractor.Content;

                currRow.Cells.Add(new Cell
                {
                    IsHeader = (cell.NodeName == "TH"),
                    Contents = contents,
                    ColSpan = ParseSpan(cell.GetAttribute("colspan")),
                    RowSpan = ParseSpan(cell.GetAttribute("rowspan")),
                    IsRowSpanHolder = false
                });
            }
        }

        //parse the value of a row or column span. Browsers are support liberal on this
        // "3;" works. Defaults to 1 if you can't parse anything
        private int ParseSpan(string attribValue)
        {
            try
            {
                if (attribValue != null)
                {
                    var match = Regex.Match(attribValue, @"^(\d+)");
                    return match.Success ? Convert.ToInt32(match.Groups[1].Value) : 1;
                }
            }
            catch (Exception)
            { }
            return 1;
        }

        private int RowWidthThrottle(int colSpan)
        {
            if(currRowWidth + colSpan <= table.MaxColumns)
            {
                currRowWidth += colSpan;
                return colSpan;
            }
            var newColspan = Math.Max((table.MaxColumns - currRowWidth), 1);
            currRowWidth += newColspan;
            return newColspan;
        }
            

        private void UpdateForRowSpans()
        {
            for (int rowIndex = 1; rowIndex < table.Rows.Count; rowIndex++)
            {
                List<Cell> newRow = new List<Cell>();
                Queue<Cell> oldRow = new Queue<Cell>(table.Rows[rowIndex].Cells);
                Queue<Cell> prevRow = new Queue<Cell>(table.Rows[rowIndex - 1].Cells);
                currRowWidth = 0;
                while (prevRow.Count > 0)
                {
                    var prevRowCell = prevRow.Dequeue();

                    if (prevRowCell.RowSpan > 1)
                    {
                        //push on a placeholder
                        newRow.Add(new Cell
                        {
                            IsRowSpanHolder = true,
                            RowSpan = prevRowCell.RowSpan - 1,
                            ColSpan = RowWidthThrottle(prevRowCell.ColSpan),
                            IsHeader = prevRowCell.IsHeader,
                        });
                    }
                    else
                    {
                        for (int i = 0; i < prevRowCell.ColSpan; i++)
                        {
                            //pull cell from current row == the colspan of 
                            if (oldRow.Count > 0)
                            {
                                var cell = oldRow.Dequeue();
                                cell.ColSpan = RowWidthThrottle(cell.ColSpan);
                                newRow.Add(cell);
                                i += cell.ColSpan - 1;
                            }
                        }
                    }
                }
                //There should not be anything left in oldRow. If so, the
                //number of cells in the source table were mismatched, so try
                //and handle that
                while (oldRow.Count > 0)
                {
                    newRow.Add(oldRow.Dequeue());
                }
                table.Rows[rowIndex].Cells = newRow;
            }
        }

    }
}
