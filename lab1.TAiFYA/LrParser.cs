using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static lab1.TAiFYA.Form1;

namespace lab1.TAiFYA
{
    public class LrParser
    {
        Stack<int> stateStack = new Stack<int>();
        Stack<string> lexemStack = new Stack<string>();
        public List<Token> tokens;
        public List<string> program;
        private bool isEnd = false;
        int countLex = 0;
        int state = 0;
        public List<string> matrix = new List<string>();

        static Dictionary<TokenType, string> StringSpecialWords = new Dictionary<TokenType, string>()
        {
            {TokenType.BEGIN,"begin" },
            {TokenType.TYPE, "integer" },
            {TokenType.BOOL, "bool"},
            {TokenType.IF, "if" },
            {TokenType.ELSE, "else"},
            {TokenType.END, "end"},
            {TokenType.VAR, "var" },
            {TokenType.THEN, "then"},
            {TokenType.AND, "and" },
            {TokenType.EQUAL, ":="},
            {TokenType.IDENTIFIER, "id"},
            {TokenType.LITERAL, "lit" },
            {TokenType.DOUBLE, "double"},
            {TokenType.FLOAT, "float"}
        };
        static Dictionary<TokenType, string> StringSpecialSymbols = new Dictionary<TokenType, string>()
        {
            {TokenType.SEMICOLON, ";"},
            {TokenType.LPAR , "(" },
            {TokenType.RPAR , ")" },
            {TokenType.PLUS , "+"},
            {TokenType.MINUS , "-"},
            {TokenType.DOT , "." },
            {TokenType.MORE , ">" },
            {TokenType.LESS , "<" },
            {TokenType.COMMA , "," },
            {TokenType.COLON , ":" }
        };

        private static bool isStringWord(Token token)
        {
            return StringSpecialWords.ContainsKey(token.Type);
        }
        private static bool isStringSymbol(Token token)
        {
            return StringSpecialSymbols.ContainsKey(token.Type);
        }

        private void Reduce(int num, string neterm)
        {
            for (int i = 0; i < num; i++)
            {
                stateStack.Pop();
                lexemStack.Pop();
            }
            lexemStack.Push(neterm);
            state = stateStack.Peek();
        }
        private void Shift()
        {
            lexemStack.Push(GetLexeme());
            countLex++;
        }

        private void GoToState(int state)
        {
            stateStack.Push(state);
            this.state = state;
        }
        private string GetLexeme()
        {
            if (isStringWord(tokens[countLex]))
            {
                return StringSpecialWords[tokens[countLex].Type];
            }
            else if (isStringSymbol(tokens[countLex]))
            {
                return StringSpecialSymbols[tokens[countLex].Type];
            }
            else throw new Exception("Такой лексемы нет в списке");
        }
        private string LookAhead(int depth)
        {
            Stack<string> lexemAheadStack = new Stack<string>();
            for (int i = 0; i < depth; i++)
            {
                lexemAheadStack.Push(GetLexeme());
                countLex++;
            }
            countLex -= depth;
            return lexemAheadStack.Peek();
        }
        Dictionary<string, int> priority = new Dictionary<string, int>()
        {
            {"(", 0  },
            {")", 1},
            {"or", 2 },
            {"and",3 },
            {"not",4 },
            {"<",5 },
            {">",5},
            {":=",5 },
            {"!=",5 },
            {"+",6 },
            {"-",6},
            {"*",7},
            {"/",7},
            {"^",8 }
        };
        Dictionary<string, int> oper = new Dictionary<string, int>()
        {
            {"or",2 },
            {"and",3 },
            {"not",4 },
            {"<",5 },
            {">",5},
            {":=",5 },
            {"!=",5 },
            {"+",6 },
            {"-",6 },
            {"*",7},
            {"/",7},
            {"^",8},
        };
        private bool isOper(string word)
        {
            return oper.ContainsKey(word);
        }
        private void Expr()
        {
            string expression = "";
            while (lexemStack.Peek() != "then")
            {
                expression += lexemStack.Pop();
                Shift();
            }
            countLex--;
            lexemStack.Pop();
            lexemStack.Push("expr");

            string outputLine = "";
            Stack<string> stackOper = new Stack<string>();
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == 'i')
                {
                    outputLine += " ";
                    outputLine += expression[i];
                    outputLine += expression[i + 1];
                    i++;
                }
                else if (expression[i] == 'l')
                {
                    outputLine += " ";
                    outputLine += expression[i];
                    outputLine += expression[i + 1];
                    outputLine += expression[i + 2];
                    i += 2;
                }

                if (expression[i] == '(')
                { stackOper.Push("("); }
                else if (expression[i] == ')')
                {
                    while (stackOper.Peek() != "(")
                    {
                        outputLine += " ";
                        outputLine += stackOper.Pop();
                        if (stackOper.Count == 0)
                        { break; throw new Exception(); }
                    }
                    stackOper.Pop();
                }

                if (isOper(expression[i].ToString()))
                {
                    while (priority[stackOper.Peek()] >= priority[expression[i].ToString()])
                    {
                        outputLine += " ";
                        outputLine += stackOper.Pop();
                    }
                    stackOper.Push(expression[i].ToString());
                }
                else if (expression[i] == 'a')
                {
                    string oper = "and";
                    i += 2;
                    if (stackOper.Count == 0)
                    { stackOper.Push(oper); }
                    else
                        while (priority[stackOper.Peek()] >= priority[oper])
                        {
                            outputLine += " ";
                            outputLine += stackOper.Pop();
                        }
                    stackOper.Push(oper);
                }
                else if (expression[i] == 'o')
                {
                    string oper = "or";
                    i++;
                    if (stackOper.Count == 0)
                    { stackOper.Push(oper); }
                    else
                        while (priority[stackOper.Peek()] >= priority[oper])
                        {
                            outputLine += " ";
                            outputLine += stackOper.Pop();
                        }
                    stackOper.Push(oper);
                }
                else if (expression[i] == 'n')
                {
                    string oper = "not";
                    i += 2;
                    if (stackOper.Count == 0)
                    { stackOper.Push(oper); }
                    else
                        while (priority[stackOper.Peek()] >= priority[oper])
                        {
                            outputLine += " ";
                            outputLine += stackOper.Pop();
                        }
                    stackOper.Push(oper);
                }
            }

            for (int i = 0; i < stackOper.Count; i++)
            {
                outputLine += " ";
                outputLine += stackOper.Pop();
            }

            OpnInMatrix(outputLine);
        }
        private void OpnInMatrix(string opn)
        {
            try
            {
                Stack<string> stackOperand = new Stack<string>();
                List<string> matrixOper = new List<string>();
                int operation = 0;
                for (int i = 0; i < opn.Length; i++)
                {
                    if (opn[i] == 'i')
                    {
                        stackOperand.Push("id");
                        i++;
                    }
                    else if (opn[i] == 'l')
                    {
                        stackOperand.Push("lit");
                        i += 2;
                    }

                    if (isOper(opn[i].ToString()))
                    {
                        string operand1 = stackOperand.Pop();
                        string operand2 = stackOperand.Pop();
                        operation++;
                        string matrixLine = "m" + operation + ": " + opn[i] + " " + operand2 + " " + operand1;
                        matrixOper.Add(matrixLine);
                        stackOperand.Push("m" + operation);
                    }
                    else if (opn[i] == 'a')
                    {
                        i += 2;
                        string operand1 = stackOperand.Pop();
                        string operand2 = stackOperand.Pop();
                        operation++;
                        string matrixLine = "m" + operation + ": " + "and" + " " + operand2 + " " + operand1;
                        matrixOper.Add(matrixLine);
                        stackOperand.Push("m" + operation);
                    }
                    else if (opn[i] == 'o')
                    {
                        i++;
                        string operand1 = stackOperand.Pop();
                        string operand2 = stackOperand.Pop();
                        operation++;
                        string matrixLine = "m" + operation + ": " + "or" + " " + operand2 + " " + operand1;
                        matrixOper.Add(matrixLine);
                        stackOperand.Push("m" + operation);
                    }
                    else if (opn[i] == 'n')
                    {
                        i += 2;
                        string operand1 = stackOperand.Pop();
                        string operand2 = stackOperand.Pop();
                        operation++;
                        string matrixLine = "m" + operation + ": " + "not" + " " + operand2 + " " + operand1;
                        matrixOper.Add(matrixLine);
                        stackOperand.Push("m" + operation);
                    }
                }
                matrix = matrixOper;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ожибка в выражении");
                throw new Exception();
            }
        }
        public void LrParserFunc()
        {
            stateStack.Push(0);
            while (!isEnd)
            {
                switch (state)
                {
                    case 0:
                        State0(); break;
                    case 1:
                        State1(); break;
                    case 2:
                        State2(); break;
                    case 3:
                        State3(); break;
                    case 4:
                        State4(); break;
                    case 5:
                        State5(); break;
                    case 6:
                        State6(); break;
                    case 7:
                        State7(); break;
                    case 8:
                        State8(); break;
                    case 9:
                        State9(); break;
                    case 10:
                        State10(); break;
                    case 11:
                        State11(); break;
                    case 12:
                        State12(); break;
                    case 13:
                        State13(); break;
                    case 14:
                        State14(); break;
                    case 15:
                        State15(); break;
                    case 16:
                        State16(); break;
                    case 17:
                        State17(); break;
                    case 18:
                        State18(); break;
                    case 19:
                        State19(); break;
                    case 20:
                        State20(); break;
                    case 21:
                        State21(); break;
                    case 22:
                        State22(); break;
                    case 23:
                        State23(); break;
                    case 24:
                        State24(); break;
                    case 25:
                        State25(); break;
                    case 26:
                        State26(); break;
                    case 27:
                        State27(); break;
                    case 28:
                        State28(); break;
                    case 29:
                        State29(); break;
                    case 30:
                        State30(); break;
                    case 31:
                        State31(); break;
                    case 32:
                        State32(); break;
                    case 33:
                        State33(); break;
                    case 34:
                        State34(); break;
                    case 35:
                        State35(); break;
                    case 36:
                        State36(); break;
                    case 37:
                        State37(); break;
                    case 38:
                        State38(); break;
                    case 39:
                        State39(); break;
                    case 40:
                        State40(); break;
                    case 41:
                        State41(); break;
                    case 42:
                        State42(); break;
                    case 43:
                        State43(); break;
                    case 44:
                        State44(); break;
                    case 45:
                        State45(); break;
                    case 46:
                        State46(); break;
                    case 47:
                        State47(); break;
                    case 48:
                        State48(); break;
                    case 49:
                        State49(); break;
                    case 50:
                        State50(); break;
                    case 51:
                        State51(); break;
                    case 52:
                        State52(); break;
                    case 53:
                        State53(); break;
                }
            }
        }
        private void HandlerErrors()
        {
            string currentLexeme = lexemStack.Pop();
            //MessageBox.Show($"состояние - {stateStack.Peek()}, проблемная лексема - {currentLexeme}");
            //throw new Exception();
            if (lexemStack.Count == 0)
            {
                MessageBox.Show($"Ожидалось var вместо {currentLexeme}");
                throw new Exception();
            }
            switch (lexemStack.Peek())
            {
                case "var":
                    MessageBox.Show($"Ожидалось id вместо {currentLexeme}");
                    throw new Exception();
                case ",":
                    MessageBox.Show($"Ожидалось id вместо {currentLexeme}");
                    throw new Exception();
                case ":":
                    MessageBox.Show($"Ожидалость type вместо {currentLexeme}");
                    throw new Exception();
                case ";":
                    lexemStack.Pop();
                    if (lexemStack.Peek() == "list_desc")
                    { MessageBox.Show($"Ожидалось begin вместо {currentLexeme}"); }
                    throw new Exception();
                case ":=":
                    MessageBox.Show($"Ожидалось id или lit вместо {currentLexeme}");
                    throw new Exception();
                case "expr":
                    MessageBox.Show($"Ожидалось then вместо {currentLexeme}");
                    throw new Exception();
                case "else":
                    MessageBox.Show($"Ожидалось begin вместо {currentLexeme}");
                    throw new Exception();
                default:
                    MessageBox.Show($"Ожидалось id вместо {currentLexeme}");
                    throw new Exception();
            }
        }
        private void State0()
        {
            if (lexemStack.Count == 0)
                Shift();
            switch (lexemStack.Peek())
            {
                case "program'":
                    isEnd = true; break;
                case "program":
                    GoToState(1); break;
                case "var":
                    GoToState(2); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка");
                    throw new Exception();
            }
        }
        private void State1()
        {
            switch (lexemStack.Peek())
            {
                case "program":
                    Reduce(1, "program'"); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка");
                    throw new Exception();
            }
        }
        private void State2()
        {
            switch (lexemStack.Peek())
            {
                case "var":
                    Shift(); break;
                case "list_desc":
                    if (LookAhead(2) == "begin") GoToState(3);
                    else { MessageBox.Show($"Ожидалось begin вместо {LookAhead(2)}"); throw new Exception(); }//GoToState(5);
                    break;
                case "desc_var":
                    GoToState(4); break;
                case "list_var":
                    if (LookAhead(1) == ":") { GoToState(6); }
                    else if (LookAhead(1) == ",") { GoToState(8); }
                    else throw new Exception();
                    break;
                case "id":
                    GoToState(7); break;
                case "cond":GoToState(13);break; //new string
                default:
                    HandlerErrors();
                    MessageBox.Show("syntax error"); throw new Exception();
            }
        }
        private void State3()
        {
            switch (lexemStack.Peek())
            {
                case "list_desc":
                    Shift(); break;
                case ";":
                    GoToState(9); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State4()
        {
            switch (lexemStack.Peek())
            {
                case "desc_var":
                    Reduce(1, "list_desc"); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State5()
        {
            switch (lexemStack.Peek())
            {
                case "list_desc":
                    Shift(); break;
                case ";":
                    GoToState(10); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State6()
        {
            switch (lexemStack.Peek())
            {
                case "list_var":
                    Shift(); break;
                case ":":
                    GoToState(11); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State7()
        {
            switch (lexemStack.Peek())
            {
                case "id":
                    Reduce(1, "list_var"); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State8()
        {
            switch (lexemStack.Peek())
            {
                case "list_var":
                    Shift(); break;
                case ",":
                    GoToState(12); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State9()
        {
            switch (lexemStack.Peek())
            {
                case ";":
                    Shift(); break;
                case "begin":
                    GoToState(13); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State10()
        {
            switch (lexemStack.Peek())
            {
                case ";": Shift(); break;
                case "desc_var":
                    GoToState(14); break;
                case "list_var":
                    if (LookAhead(1) == ":") { GoToState(6); }
                    else if (LookAhead(1) == ",") { GoToState(8); }
                    break;
                case "id": GoToState(7); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State11()
        {
            switch (lexemStack.Peek())
            {
                case ":":
                    Shift(); break;
                case "type":
                    GoToState(15); break;
                case "integer":
                    GoToState(16); break;
                case "double":
                    GoToState(16); break;
                case "float":
                    GoToState(16); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State12()
        {
            switch (lexemStack.Peek())
            {
                case ",": Shift(); break;
                case "id": GoToState(17); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State13()
        {
            switch (lexemStack.Peek())
            {
                case ";": Shift(); break;
                case "begin": Shift(); break;
                case "list_oper":
                    if (LookAhead(2) == "end") GoToState(18);
                    else if (LookAhead(2) == "id") GoToState(20);
                    else if (LookAhead(2) == "if") Shift();
                    else throw new Exception();
                    break;
                case "oper":
                    GoToState(19); break;
                case "cond":
                    GoToState(21); break;
                case "assign": GoToState(22); break;
                case "if": GoToState(23); break;
                case "id":
                    if ((LookAhead(3) == "+") || (LookAhead(3) == "-")
                        || (LookAhead(3) == "*") || (LookAhead(3) == "/")) GoToState(24);
                    else GoToState(25);
                    break;
                default:
                    HandlerErrors();
                    throw new Exception();
            }
        }
        private void State14()
        {
            switch (lexemStack.Peek())
            {
                case "desc_var":
                    Reduce(3, "list_desc");
                    break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State15()
        {
            switch (lexemStack.Peek())
            {
                case "type":
                    Reduce(3, "desc_var");
                    break;
                default:
                    HandlerErrors();
                    MessageBox.Show($"Ожидалось type вместо {lexemStack.Peek()}"); 
                    throw new Exception();
            }
        }
        private void State16()
        {
            switch (lexemStack.Peek())
            {
                case "integer":
                    Reduce(1, "type");
                    break;
                case "double":
                    Reduce(1, "type");
                    break;
                case "float":
                    Reduce(1, "type");
                    break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State17()
        {
            if (lexemStack.Peek() == "id")
            { Reduce(3, "list_var"); }
            else { MessageBox.Show($"Ожидалось id вместо {lexemStack.Peek()}");
                throw new Exception(); }
        }
        private void State18()
        {
            switch (lexemStack.Peek())
            {
                case "list_oper": Shift(); break;
                case ";": GoToState(26); break;
                default:
                    HandlerErrors();
                    MessageBox.Show($"Ожидалось list_oper вместо {lexemStack.Peek()}"); 
                    throw new Exception();
            }
        }
        private void State19()
        {
            switch (lexemStack.Peek())
            {
                case "oper":
                    Reduce(1, "list_oper");
                    break;
                default:
                    MessageBox.Show($"Ожидалось oper вместо {lexemStack.Peek()}");
                    throw new Exception();
            }
        }
        private void State20()
        {
            switch (lexemStack.Peek())
            {
                case "list_oper": Shift(); break;
                case ";": GoToState(27); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State21()
        {
            switch (lexemStack.Peek())
            {
                case "cond": Reduce(1, "oper"); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State22()
        {
            switch (lexemStack.Peek())
            {
                case "assign":
                    Reduce(1, "oper"); break;
                default:
                    HandlerErrors();
                    break;
            }
        }
        private void State23()
        {
            switch (lexemStack.Peek())
            {
                case "if": Shift(); break;
                case "expr": GoToState(28); break;
                case "(": Expr(); break;
                case "block_oper": GoToState(47);break; //new string
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State24()
        {
            switch (lexemStack.Peek())
            {
                case "id": Shift(); break;
                case ":=": GoToState(29); break;
                case "+": GoToState(34);break;
                case "-": GoToState(34);break;
                case "*": GoToState(34);break;
                case "/": GoToState(34);break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State25()
        {
            switch (lexemStack.Peek())
            {
                case "id":
                    Shift(); break;
                case ":=":
                    GoToState(30); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State26()
        {
            switch (lexemStack.Peek())
            {
                case ";": Shift(); break;
                case "end": GoToState(31); break;
                default:
                    string currentLexeme = lexemStack.Pop();
                    if (lexemStack.Peek() == ";")
                    { MessageBox.Show($"Ожидалось end вместо {currentLexeme}"); }
                    else { MessageBox.Show($"Ожидалось ; вместо {currentLexeme}"); }
                    throw new Exception();
            }
        }
        private void State27()
        {
            switch (lexemStack.Peek())
            {
                case ";": Shift(); break;
                case "oper": GoToState(32); break;
                case "cond": GoToState(21); break;
                case "assign": GoToState(22); break;
                case "id": GoToState(25); break;
                case "if": GoToState(23); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State28()
        {
            switch (lexemStack.Peek())
            {
                case "expr": Shift(); break;
                case "then": GoToState(33); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State29()
        {
            switch (lexemStack.Peek())
            {
                case ":=": Shift(); break;
                case "variable":
                    GoToState(34);
                    break;
                case "id": GoToState(35); break;
                case "lit": GoToState(36); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State30()
        {
            switch (lexemStack.Peek())
            {
                case ":=": Shift(); break;
                case "variable": GoToState(37); break;
                case "id": GoToState(35); break;
                case "lit": GoToState(36); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); 
                    throw new Exception();
            }
        }
        private void State31()
        {
            switch (lexemStack.Peek())
            {
                case "end": Shift(); break;
                case ".": GoToState(38); break;
                default:
                    string currentLexeme = lexemStack.Pop();
                    if (lexemStack.Peek() == "end")
                    { MessageBox.Show($"Ожидалось . вместо {currentLexeme}"); }
                    else { MessageBox.Show($"Ожидалось end вместо {currentLexeme}"); }
                    throw new Exception();
            }
        }
        private void State32()
        {
            switch (lexemStack.Peek())
            {
                case "oper":
                    Reduce(3, "list_oper");
                    break;
                default:
                    MessageBox.Show($"Ожидалось oper вместо {lexemStack.Peek()}"); 
                    throw new Exception();
            }
        }
        private void State33()
        {
            switch (lexemStack.Peek())
            {
                case "then": Shift(); break;
                case "block_oper":
                    GoToState(39);
                    break;
                case "oper": GoToState(40); break;
                case "begin": GoToState(41); break;
                case "cond": GoToState(21); break;
                case "assign": GoToState(22); break;
                case "if": GoToState(23); break;
                case "id":
                    GoToState(25); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State34()
        {
            switch (lexemStack.Peek())
            {
                case "variable":
                    Shift(); break;
                case "sign":
                    GoToState(42); break;
                case "+":
                    GoToState(43); break;
                case "-":
                    GoToState(44); break;
                case "*":
                    GoToState(45); break;
                case "/":
                    GoToState(46); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State35()
        {
            switch (lexemStack.Peek())
            {
                case "id": Reduce(1, "variable"); break;
                default:
                    MessageBox.Show($"Ожидалось id вместо {lexemStack.Peek()}"); 
                    throw new Exception();
            }
        }
        private void State36()
        {
            switch (lexemStack.Peek())
            {
                case "lit": Reduce(1, "variable"); break;
                default:
                    MessageBox.Show($"Ожидалось literal вместо {lexemStack.Peek()}"); 
                    throw new Exception();
            }
        }
        private void State37()
        {
            switch (lexemStack.Peek())
            {
                case "variable":
                    Reduce(3, "assign");
                    break;
                default:
                    MessageBox.Show($"Ожидалось variable {lexemStack.Peek()}"); 
                    throw new Exception();
            }
        }
        private void State38()
        {
            switch (lexemStack.Peek())
            {
                case ".":
                    Reduce(8, "program");
                    break;
                default:
                    MessageBox.Show($"Ожидалось . вместо {lexemStack.Peek()}"); 
                    throw new Exception();
            }
        }
        private void State39()
        {
            switch (lexemStack.Peek())
            {
                case "block_oper":Shift(); break;
                case ";": Shift(); break; //new string
                case "alt": GoToState(47); break;
                case "else": GoToState(48); break;
                case "e": Reduce(1, "alt"); break;
                case "end": Reduce(3, "block_oper");break; //new string
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State40()
        {
            switch (lexemStack.Peek())
            {
                case "oper": Reduce(1, "block_oper"); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State41()
        {
            switch (lexemStack.Peek())
            {
                case "begin": Shift(); break;
                case "list_oper":
                    if (LookAhead(2) == "id") GoToState(20);
                    else GoToState(49);
                    break;
                case "oper": GoToState(19); break;
                case "cond": GoToState(21); break;
                case "assign": GoToState(22); break;
                case "if": GoToState(23); break;
                case "id":
                    if ((LookAhead(3) == "+") || (LookAhead(3) == "-")
                        || (LookAhead(3) == "*") || (LookAhead(3) == "/")) GoToState(24);
                    else GoToState(25);
                    break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State42()
        {
            switch (lexemStack.Peek())
            {
                case "sign": Shift(); break;
                case "variable": GoToState(50); break;
                case "id": GoToState(35); break;
                case "lit": GoToState(36); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State43()
        {
            switch (lexemStack.Peek())
            {
                case "+": Reduce(1, "sign"); break;
                default:
                    MessageBox.Show($"Ожидалось + вместо {lexemStack.Peek()}"); throw new Exception();
            }
        }
        private void State44()
        {
            switch (lexemStack.Peek())
            {
                case "-":
                    Reduce(1, "sign"); break;
                default:
                    MessageBox.Show($"Ожидалось - вместо {lexemStack.Peek()}");
                    throw new Exception();
            }
        }
        private void State45()
        {
            switch (lexemStack.Peek())
            {
                case "*": Reduce(1, "sign"); break;
                default:
                    MessageBox.Show($"Ожидалось * вместо {lexemStack.Peek()}");
                    throw new Exception();
            }
        }
        private void State46()
        {
            switch (lexemStack.Peek())
            {
                case "/": Reduce(1, "sign"); break;
            }
        }
        private void State47()
        {
            switch (lexemStack.Peek())
            {
                case "alt": Reduce(5, "cond"); break;
                case "block_oper": Reduce(5, "cond");break; //new string
                default:
                    MessageBox.Show($"Ожидалось alt вместо {lexemStack.Peek()}"); 
                    throw new Exception();
            }
        }
        private void State48()
        {
            switch (lexemStack.Peek())
            {
                case "else": Shift(); break;
                case "block_oper": GoToState(51); break;
                case "oper": GoToState(40); break;
                case "begin": GoToState(41); break;
                default:
                    HandlerErrors();
                    MessageBox.Show("Синтаксическая ошибка"); throw new Exception();
            }
        }
        private void State49()
        {
            switch (lexemStack.Peek())
            {
                case "list_oper": Shift(); break;
                case ";": GoToState(52); break;
                default:
                    MessageBox.Show($"Ожидалось list_oper или ; вместо {lexemStack.Peek()}"); 
                    throw new Exception();
            }
        }
        private void State50()
        {
            switch (lexemStack.Peek())
            {
                case "variable":
                    Reduce(5, "assign"); break;
                default:
                    MessageBox.Show($"Ожидалось variable вместо {lexemStack.Peek()}"); 
                    throw new Exception();
            }
        }
        private void State51()
        {
            switch (lexemStack.Peek())
            {
                case "block_oper":
                    Reduce(2, "alt"); break;
                default:
                    MessageBox.Show($"Ожидалось block_oper вместо {lexemStack.Peek()}"); 
                    throw new Exception();
            }
        }
        private void State52()
        {
            switch (lexemStack.Peek())
            {
                case ";": Shift(); break;
                case "end": GoToState(53); break;
                default:
                    MessageBox.Show($"Ожидалось end или ; вместо {lexemStack.Peek()}");
                    throw new Exception();
            }
        }
        private void State53()
        {
            switch (lexemStack.Peek())
            {
                case "end": Reduce(4, "block_oper"); break;
                default:
                    MessageBox.Show($"Ожидалось end вместо {lexemStack.Peek()}");
                    throw new Exception();
            }
        }
    }
}
