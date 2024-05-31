using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static lab1.TAiFYA.Form1;

namespace lab1.TAiFYA
{
    public class LkParser
    {
        public List<Token> tokens;
        public List<string> program;
        public Token currentLexeme;
        public int count = 0;

        public void Operator()
        {
            switch (currentLexeme.Type)
            {
                case TokenType.SEMICOLON:
                    Semicolon(); break;
                case TokenType.VAR:
                    Declaration(); break;
                case TokenType.TYPE:
                    Types(); break;
                case TokenType.BEGIN:
                    Begin(); break;
                case TokenType.IDENTIFIER:
                    Assigment(); break;
                case TokenType.IF:
                    Condition(); break;
                case TokenType.AND:
                    MultipleCondition(); break;
                case TokenType.THEN:
                    Then(); break;
                case TokenType.ELSE:
                    Else(); break;
                case TokenType.END:
                    End(); break;
                case TokenType.DOT:
                    Dot(); break;
                case TokenType.LPAR:
                    CheckBrackets(); break;
            }
        }
        private void CheckBrackets()
        {
            Next();
            while (currentLexeme.Type != TokenType.RPAR)
            {
                if (currentLexeme.Type != TokenType.IDENTIFIER)
                {
                    if (currentLexeme.Type != TokenType.MORE)
                    {
                        if (currentLexeme.Type != TokenType.LESS)
                        {
                            if (currentLexeme.Type != TokenType.EQUAL)
                            {
                                if (currentLexeme.Type != TokenType.LITERAL)
                                    throw new Exception($"Ожидалось RPAR, вместо {currentLexeme.Type}");
                            }
                        }
                    }
                }
                Next();
            }
            Next();
            if (currentLexeme.Type != TokenType.AND)
                if (currentLexeme.Type != TokenType.THEN)
                    throw new Exception($"Ожидалось AND или THEN, вместо {currentLexeme.Type}");
            Operator();
        }
        private void Dot()
        {
            if (currentLexeme.Type != TokenType.DOT)
                throw new Exception($"Ожидалось DOT, вместо {currentLexeme.Type}");
            //textBox1.Text = "Парсер прошел успешно";
        }
        private void End()
        {
            if (currentLexeme.Type != TokenType.END)
                throw new Exception($"Ожидалось END, вместо {currentLexeme.Type}");
            Next();
            if (currentLexeme.Type != TokenType.SEMICOLON)
                if (currentLexeme.Type != TokenType.DOT)
                    throw new Exception($"Ожидалось ';' или '.', вместо {currentLexeme.Type}");
            Operator();
        }
        private void Else()
        {
            if (currentLexeme.Type != TokenType.ELSE)
                throw new Exception($"Ожидалось ELSE, вместо {currentLexeme.Type}");
            Next();
            if (currentLexeme.Type != TokenType.BEGIN)
                throw new Exception($"Ожидалось BEGIN, вместо {currentLexeme.Type}");
            Operator();
        }
        private void Then()
        {
            if (currentLexeme.Type != TokenType.THEN)
                throw new Exception($"Ожидалось THEN, вместо {currentLexeme.Type}");
            Next();
            Operator();
        }
        private void MultipleCondition()
        {
            if (currentLexeme.Type != TokenType.AND)
                throw new Exception($"Ожидалось AND, вместо {currentLexeme.Type}");
            Next();
            if (currentLexeme.Type != TokenType.LPAR)
                throw new Exception($"Ожидалось LPAR, вместо {currentLexeme.Type}");
            Operator();
        }
        private void Condition()
        {
            if (currentLexeme.Type != TokenType.IF)
                throw new Exception($"Ожидалось IF, вместо {currentLexeme.Type}");
            Next();
            if (currentLexeme.Type != TokenType.LPAR)
                throw new Exception($"Ожидалось LPAR, вместо {currentLexeme.Type}");
            Operator();
        }
        private void Assigment()
        {
            if (currentLexeme.Type != TokenType.IDENTIFIER)
                throw new Exception($"Ожидалось ID, вместо {currentLexeme.Type}");
            else if (currentLexeme.Type != TokenType.EQUAL)
                throw new Exception($"Ожидалось EQUAL, вместо {currentLexeme.Type}");
            else if (currentLexeme.Type != TokenType.LITERAL)
                throw new Exception($"Ожидалось LITERAL, вместо {currentLexeme.Type}");
            Next();
            Operator();
        }
        private void Semicolon()
        {
            if (currentLexeme.Type != TokenType.SEMICOLON)
                throw new Exception($"Ожидалось SEMICOLON, вместо {currentLexeme.Type}");
            Next();
            Operator();
        }

        private void Begin()
        {
            if (currentLexeme.Type != TokenType.BEGIN)
                throw new Exception($"Ожидалось BEGIN, вместо {currentLexeme.Type}");
            Next();
            bool endexists = false;
            for (int i = count; i < tokens.Count; i++)
            {
                if (tokens[i].Type == TokenType.END)
                {
                    endexists = true;
                    break;
                }
            }
            if (!endexists)
                throw new Exception($"Ожидалось END, вместо {currentLexeme.Type}");
            Operator();
        }
        private void Declaration()
        {
            if (currentLexeme.Type != TokenType.VAR)
                throw new Exception($"Ожидалось VAR, вместо {currentLexeme.Type}");
            Next();
            if (currentLexeme.Type != TokenType.IDENTIFIER)
                throw new Exception($"Ожидалось ID, вместо {currentLexeme.Type}");
            Next();
            Identifiers();
        }

        private void Identifiers()
        {
            if (currentLexeme.Type == TokenType.COMMA)
            {
                Next();
                if (currentLexeme.Type != TokenType.IDENTIFIER)
                    throw new Exception($"Ожидалось ID, вместо {currentLexeme.Type}");
                Next();
                Identifiers();
            }
            else if (currentLexeme.Type == TokenType.COLON)
            {
                Next();
                Operator();
            }
            else throw new Exception($"Ожидалось COLON, вместо {currentLexeme.Type}");
        }

        private void Types()
        {
            if (currentLexeme.Type != TokenType.TYPE)
                throw new Exception($"Ожидалось TYPE, вместо {currentLexeme.Type}");
            Next();
            if (currentLexeme.Type != TokenType.SEMICOLON)
                throw new Exception($"Ожидалось SEMICOLON, вместо {currentLexeme.Type}");
            Operator();
        }
        private void Next()
        {
            if (count < tokens.Count)
            {
                count++;
                currentLexeme = tokens[count];
            }
            else { throw new Exception("Программа завершилась"); }
        }
    }
}
