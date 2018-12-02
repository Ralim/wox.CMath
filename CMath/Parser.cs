using System;
using System.Collections.Generic;
using System.Text;

namespace CMath
{
    public class Parser
    {
        public const char START_ARG = '(';
        public const char END_ARG = ')';
        public const char END_LINE = '\n';

        class Cell
        {
            internal Cell(double value, char action)
            {
                Value = value;
                Action = action;
            }

            internal double Value { get; set; }
            internal char Action { get; set; }
        }

        public static double process(string data)
        {
            // Get rid of spaces and check parenthesis
            string expression = preprocess(data);
            int from = 0;

            return loadAndCalculate(data, ref from, END_LINE);
        }

        static string preprocess(string data)
        {
            if (string.IsNullOrEmpty(data)) {
                throw new ArgumentException("Loaded empty data");
            }

            int parentheses = 0;
            StringBuilder result = new StringBuilder(data.Length);

            for (int i = 0; i < data.Length; i++) {
                char ch = data[i];
                switch (ch) {
                    case ' ':
                    case '\t':
                    case '\n': continue;
                    case END_ARG:
                        parentheses--;
                        break;
                    case START_ARG:
                        parentheses++;
                        break;
                }
                result.Append(ch);
            }

            if (parentheses != 0) {
                throw new ArgumentException("Uneven parenthesis");
            }

            return result.ToString();
        }

        public static double loadAndCalculate(string data, ref int from, char to = END_LINE)
        {
            if (from >= data.Length || data[from] == to) {
                throw new ArgumentException("Loaded invalid data: " + data);
            }

            List<Cell> listToMerge = new List<Cell>(16);
            StringBuilder item = new StringBuilder();

            do { // Main processing cycle of the first part.
                char ch = data[from++];
                if (stillCollecting(item.ToString(), ch, to)) { // The char still belongs to the previous operand.
                    item.Append(ch);
                    if (from < data.Length && data[from] != to) {
                        continue;
                    }
                }

                // We are done getting the next token. The getValue() call below may
                // recursively call loadAndCalculate(). This will happen if extracted
                // item is a function or if the next item is starting with a START_ARG '('.
                ParserFunction func = new ParserFunction(data, ref from, item.ToString(), ch);
                double value = func.getValue(data, ref from);

                char action = validAction(ch) ? ch
                                              : updateAction(data, ref from, ch, to);

                listToMerge.Add(new Cell(value, action));
                item.Clear();

            } while (from < data.Length && data[from] != to);

            if (from < data.Length &&
               (data[from] == END_ARG || data[from] == to)) { // This happens when called recursively: move one char forward.
                from++;
            }

            Cell baseCell = listToMerge[0];
            int index = 1;

            return merge(baseCell, ref index, listToMerge);
        }

        static bool stillCollecting(string item, char ch, char to)
        {
            // Stop collecting if either got END_ARG ')' or to char, e.g. ','.
            char stopCollecting = (to == END_ARG || to == END_LINE) ?
                                   END_ARG : to;
            return (item.Length == 0 && (ch == '-' || ch == END_ARG)) ||
                  !(validAction(ch) || ch == START_ARG || ch == stopCollecting);
        }

        static bool validAction(char ch)
        {
            return ch == '*' || ch == '/' || ch == '+' || ch == '-' || ch == '^' || ch == '#' || ch == '$' || ch == '>' || ch == '<' || ch == '|' || ch == '&' || ch == '%';
        }

        static char updateAction(string item, ref int from, char ch, char to)
        {
            if (from >= item.Length || item[from] == END_ARG || item[from] == to) {
                return END_ARG;
            }

            int index = from;
            char res = ch;
            while (!validAction(res) && index < item.Length) { // Look for the next character in string until a valid action is found.
                res = item[index++];
            }

            from = validAction(res) ? index
                                    : index > from ? index - 1
                                                   : from;
            return res;
        }

        // From outside this function is called with mergeOneOnly = false.
        // It also calls itself recursively with mergeOneOnly = true, meaning
        // that it will return after only one merge.
        static double merge(Cell current, ref int index, List<Cell> listToMerge,
                     bool mergeOneOnly = false)
        {
            while (index < listToMerge.Count) {
                Cell next = listToMerge[index++];

                while (!canMergeCells(current, next)) { // If we cannot merge cells yet, go to the next cell and merge
                                                        // next cells first. E.g. if we have 1+2*3, we first merge next
                                                        // cells, i.e. 2*3, getting 6, and then we can merge 1+6.
                    merge(next, ref index, listToMerge, true /* mergeOneOnly */);
                }
                mergeCells(current, next);
                if (mergeOneOnly) {
                    return current.Value;
                }
            }

            return current.Value;
        }

        static void mergeCells(Cell leftCell, Cell rightCell)
        {
            switch (leftCell.Action) {
                case '^':
                    leftCell.Value = Math.Pow(leftCell.Value, rightCell.Value);
                    break;
                case '*':
                    leftCell.Value *= rightCell.Value;
                    break;
                case '/':
                    if (rightCell.Value == 0) {
                        throw new ArgumentException("Division by zero");
                    }
                    leftCell.Value /= rightCell.Value;
                    break;
                case '+':
                    leftCell.Value += rightCell.Value;
                    break;
                case '#': // left shift
                    leftCell.Value = ((long)leftCell.Value) << ((int)rightCell.Value);
                    break;
                case '$'://right shift
                    leftCell.Value = ((long)leftCell.Value) >> ((int)rightCell.Value);
                    break;
                case '|'://OR
                    leftCell.Value = ((int)leftCell.Value) | ((int)rightCell.Value);
                    break;
                case '%':// Mod
                    leftCell.Value = ((int)leftCell.Value) % ((int)rightCell.Value);
                    break;
                case '<':
                    leftCell.Value = leftCell.Value < rightCell.Value ? 1 : 0;
                    break;
                case '>':
                    leftCell.Value = leftCell.Value > rightCell.Value ? 1 : 0;
                    break;
                case '-':
                    leftCell.Value -= rightCell.Value;
                    break;
            }
            leftCell.Action = rightCell.Action;
        }

        static bool canMergeCells(Cell leftCell, Cell rightCell)
        {
            return getPriority(leftCell.Action) >= getPriority(rightCell.Action);
        }

        static int getPriority(char action)
        {
            switch (action) {
                case '^': return 4;
                case '*':
                case '#':
                case '$':
                case '|':
                case '%':
                case '/': return 3;
                case '+':
                case '<':
                case '>':
                case '-': return 2;
            }
            return 0;
        }
    }
}
