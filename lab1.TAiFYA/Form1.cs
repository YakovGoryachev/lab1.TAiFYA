using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace lab1.TAiFYA
{
    public partial class Form1 : Form
    {
        OpenFileDialog dlg = new OpenFileDialog();
        lexAnalysator analysis = new lexAnalysator();
        List<string> program = new List<string>();
        List<Token> tokens = new List<Token>();
        LkParser LkParser = new LkParser();
        LrParser LrParser = new LrParser();
        public Form1()
        {
            InitializeComponent();
            analysis.program = program;
            analysis.tokens = tokens;
            LkParser.program = program;
            LkParser.tokens = tokens;
            LrParser.program = program;
            LrParser.tokens = tokens;
            analysis.dlg = dlg;
        }

        public enum TokenType
        {
            TYPE, BOOL, LITERAL, NUMBER, IDENTIFIER, BEGIN, END, IF, ELSE,
            TRUE, FALSE, LPAR, RPAR, PLUS, MINUS, EQUAL, SEMICOLON, THEN, VAR, DOT, AND, MORE, LESS,
            COLON, COMMA, DOUBLE, FLOAT
        }

        static TokenType[] Delimiters = new TokenType[]
        {
            TokenType.SEMICOLON, TokenType.PLUS, TokenType.MINUS, TokenType.EQUAL,
            TokenType.LPAR, TokenType.RPAR
        };

        public static bool IsDelimiter(Token token)
        {
            return Delimiters.Contains(token.Type);
        }

        public static Dictionary<string, TokenType> SpecialWords = new Dictionary<string, TokenType>()
        {
            {"begin", TokenType.BEGIN },
            {"integer", TokenType.TYPE },
            {"bool", TokenType.BOOL },
            {"if", TokenType.IF },
            {"else", TokenType.ELSE },
            {"end", TokenType.END },
            {"var", TokenType.VAR },
            {"then", TokenType.THEN },
            {"and", TokenType.AND },
            {":=", TokenType.EQUAL },
            {"double", TokenType.DOUBLE },
            {"float", TokenType.FLOAT }
        };

        public static bool IsSpecialWord(string word)
        {
            if (string.IsNullOrEmpty(word)) return false;
            return SpecialWords.ContainsKey(word);
        }

        public static Dictionary<char, TokenType> SpecialSymbols = new Dictionary<char, TokenType>()
        {
            {';', TokenType.SEMICOLON},
            {'(', TokenType.LPAR },
            {')', TokenType.RPAR },
            {'+', TokenType.PLUS},
            {'-', TokenType.MINUS},
            {'.', TokenType.DOT },
            {'>', TokenType.MORE },
            {'<', TokenType.LESS },
            {',', TokenType.COMMA },
            {':', TokenType.COLON }
        };

        public static bool IsSpecialSymbol(char ch)
        {
            return SpecialSymbols.ContainsKey(ch);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //ReadProgramm();
            //LexicalAnalysis();
            analysis.ReadProgramm();
            for (int i = 0; i < program.Count; i++)
            {
                listBox1.Items.Add(program[i]);
            }
            analysis.LexicalAnalysis();
            OutputTokens();
        }
        private void OutputTokens()
        {
            foreach (Token token in tokens)
            {
                listBox2.Items.Add(token);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LkParser.currentLexeme = tokens[LkParser.count];
            LkParser.Operator();
            textBox1.Text = "LK parser pass successfully";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            LrParser.LrParserFunc();
            listBox3.Items.Clear();
            foreach (var len in LrParser.matrix)
            { listBox3.Items.Add(len); }
            textBox1.Text = "LR парсер прошел успешно";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dlg.Filter = "файлы .txt | *.txt";
            dlg.ShowDialog();
            string path = dlg.FileName;
        }
    }
}
