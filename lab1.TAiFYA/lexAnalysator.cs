using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static lab1.TAiFYA.Form1;
using System.Windows.Forms;
using System.Diagnostics.Eventing.Reader;

namespace lab1.TAiFYA
{
    public class lexAnalysator
    {
        public OpenFileDialog dlg;
        public List<string> program;
        public List<Token> tokens;
        private bool isLiteral;
        private bool logical;
        public void ReadProgramm()
        {
            //OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Filter = "файлы .txt | *.txt";
            //dlg.ShowDialog();
            //string path = dlg.FileName;
            string path = "C:\\Users\\yakov\\OneDrive\\Рабочий стол\\program for parser.txt";
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    program.Add(line);
                }
            }
            //for (int i = 0; i < program.Count; i++)
            //{
            //    listBox1.Items.Add(program[i]);
            //}
        }
        public void LexicalAnalysis()
        {
            for (int i = 0; i < program.Count; i++)
            {
                string line = program[i];
                string buffer = "";
                for (int k = 0; k < line.Length; k++)
                {
                    if (IsSpecialWord(buffer))
                    {
                        Token token = new Token(SpecialWords[buffer]);
                        tokens.Add(token);
                    }
                    if (IsSpecialSymbol(line[k]) || line[k] == ' ' || line[k] == '\n')
                    {
                        CheckBuffer(ref buffer);
                        if (IsSpecialSymbol(line[k]))
                        {
                            if (line[k] == ':' && line[k + 1] == '=')
                            {
                                isLiteral = true;
                                Token token1 = new Token(TokenType.EQUAL);
                                tokens.Add(token1);
                                k++;
                            }
                            else if (line[k] == '>' || line[k] == '<')
                            {
                                logical = true;
                                Token token = new Token(SpecialSymbols[line[k]]);
                                tokens.Add(token);
                            }
                            else if (line[k] == '+' || line[k] == '-' || line[k] == '*' || line[k] == '/')
                            {
                                logical = true;
                                Token token = new Token(SpecialSymbols[line[k]]);
                                tokens.Add(token);
                            }
                            else
                            {
                                Token token = new Token(SpecialSymbols[line[k]]);
                                tokens.Add(token);
                            }
                            
                        }
                        buffer = "";
                    }
                    else buffer += line[k];
                    if (IsSpecialWord(buffer))
                    {
                        Token token = new Token(SpecialWords[buffer]);
                        tokens.Add(token);
                        buffer = "";
                    }
                }
            }
        }

        /// <summary>
        /// проверка на идентификатор или литерал
        /// </summary>
        /// <param name="buffer"></param>
        private void CheckBuffer(ref string buffer)
        {
            if (buffer != "" && !IsSpecialWord(buffer))
            {
                if (!isLiteral)
                {
                    if (char.IsDigit(buffer[0]) && !logical)
                    {
                        MessageBox.Show("Идентификатор не может начинаться с цифры");
                        throw new Exception();
                    }
                    else if (char.IsDigit(buffer[0]) && logical)
                    {
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            if (char.IsLetter(buffer[i]))
                            {
                                MessageBox.Show("Неверное значение литерала");
                                throw new Exception();
                            }
                        }
                        Token token = new Token(TokenType.LITERAL);
                        token.Value = buffer;
                        tokens.Add(token);
                    }
                    else
                    {
                        if (buffer.Length > 8)
                        {
                            MessageBox.Show("В названии идентификатора не может быть больше 8 символов");
                            throw new Exception();
                        }
                        Token token = new Token(TokenType.IDENTIFIER);
                        token.Value = buffer;
                        tokens.Add(token);
                    }
                }
                else if (isLiteral)
                {
                    isLiteral = false;
                    bool literal = false;
                    if (char.IsDigit(buffer[0]))
                    {
                        literal = true;
                    }
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        if (!char.IsDigit(buffer[i]) && literal)
                        {
                            MessageBox.Show("Неверное значение литерала");
                            throw new Exception();
                        }
                    }
                    if (literal)
                    {
                        Token token = new Token(TokenType.LITERAL);
                        token.Value = buffer;
                        tokens.Add(token);
                    }
                    else
                    {
                        if (buffer.Length > 8)
                        {
                            MessageBox.Show("В названии идентификатора не может быть больше 8 символов");
                            throw new Exception();
                        }
                        Token token = new Token(TokenType.IDENTIFIER);
                        token.Value = buffer;
                        tokens.Add(token);
                    }
                }
                if (logical) logical = false;
                buffer = "";
            }
        }
    }
}
