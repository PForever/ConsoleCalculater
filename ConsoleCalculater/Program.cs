using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace AbstractTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = "5*(x-2)+3";
            var charLst = new[] { '*', '/', '+', '-', '(', ')' };
            var valueList = new List<string>();
            var operationList = new List<char>();

            var str = "-sin(-3log(- 3 sin -1.5157 x ^ 3.5x / -x ^ 25.27 log-x)(18x + 3.87)/x^-20)";//"- 1 * - sin - 4 ^ - 2";
            double dbX = -0.395;
            Console.WriteLine($"x = {dbX}\n{str} = {Calc.Start(str, dbX)}");
            try
            {
                //Console.WriteLine(new Add(new Umn(new Const(5), new Min(new Del(new Variable(), new Const(0)), new Const(2))), new Const(3)).SetIntValue(5));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            Console.ReadLine();
        }
    }

    class Calc
    {
        private static readonly char UnoMinus = '-';
        private static readonly char OpenBracket = '(';
        private static readonly char CloseBracket = ')';

        private static readonly char CharVariable = 'x';
        private static readonly char[] CharList = { '*', '/', '+', '-', '^', '(', ')' };
        private static readonly char[] CharGroup0 = { '+', '-' };
        private static readonly char[] CharGroup1 = { '*', '/' };
        private static readonly char[] CharGroup2 = { '^' };
        private static readonly string[] StrGroup = { "sin", "cos", "log"};
        private enum MyEnum {nun, sin, cos, log, min};


        public static double Start(string startValue, double xValue)
        {
            var node = (new LinkedList<char>(startValue.Where(ch => ch != ' '))).First;
            return StrResult(default(char), ref node, out char value).SetIntValue(xValue);
        }

        private static IOperand StrResult(char preOperand, ref LinkedListNode<char> sValue, out char lastOperand)
        {
            var a = FindValue(ref sValue);
            var operand = FindOperand(ref sValue, out int operandPreor);

            if (operand == CloseBracket || preOperand != default(char) && operandPreor < 2)
            {
                lastOperand = operand;
                return a;
            }

            do
            {
                var b = FindValue(ref sValue);
                var nextOperand = FindOperand(ref sValue, out int nextOperandPreor);


                if (operandPreor >= nextOperandPreor)
                {

                    a = DuoOperand(a, b, operand);
                    operand = nextOperand;
                }
                else
                {
                    if (operandPreor == 1)
                    {
                        a = DuoOperand(a, DuoOperand(b, StrResult(operand, ref sValue, out char tempOperand), nextOperand), operand); // TODO break???
                        operand = tempOperand;
                    }
                    else
                    {
                        a = DuoOperand(a, StrResult(default(char), ref sValue, out nextOperand), operand);
                    }
                }
            } while (operand != CloseBracket && sValue != null);
            lastOperand = default(char);
            return a;
        }

        private static char FindOperand(ref LinkedListNode<char> charter, out int intPreor)
        {
            if (charter == null)
            {
                intPreor = 0;
                return default(char);
            }
            switch (charter.Value)
            {
                case '+':
                case '-':
                    charter = charter.Next;
                    intPreor = 0;
                    return charter.Previous.Value;
                case '*':
                case '/':
                    charter = charter.Next;
                    intPreor = 1;
                    return charter.Previous.Value;
                case '^':
                    charter = charter.Next;
                    intPreor = 2;
                    return charter.Previous.Value;
                case ')':
                    var temp = charter.Value;
                    if(charter.Next != null) charter = charter.Next;
                    intPreor = -1;
                    return temp;
                default:
                    intPreor = 1;
                    return '*';
            }
        }
        private static IOperand FindValue(ref LinkedListNode<char> charter)
        {
            var blUno1 = false;
            var blUno2 = false;
            IOperand aValue;
            var strNumbs = "";
            if (charter.Value == UnoMinus)
            {
                blUno1 = true;
                charter = charter.Next;
            }
            FindFunc(ref charter, out MyEnum funcValue);
            if (charter.Value == OpenBracket)
            {
                charter = charter.Next;
                aValue = StrResult(default(char), ref charter, out char tempOperand);
            }
            else
            {

                if (charter.Value == UnoMinus)
                {
                    blUno2 = true;
                    charter = charter.Next;
                }

                while (charter != null && NumbersPredicate(charter.Value))
                {
                    strNumbs += charter.Value;
                    charter = charter.Next;
                }
                if (charter?.Value == CharVariable)
                {
                    aValue = new Umn(new Const(strNumbs == "" ? 1 : Double.Parse(strNumbs.Replace('.', ','))),
                        new Variable());
                    charter = charter.Next;
                }
                else
                {
                    if (strNumbs == "") throw new Exception("Некорректная запись");
                    aValue = new Const(strNumbs == "" ? 1 : Double.Parse(strNumbs.Replace('.', ',')));
                }
                if (blUno2) aValue = UnoOperand(aValue, MyEnum.min);
            }
            aValue = UnoOperand(aValue, funcValue);
            if (blUno1) aValue = UnoOperand(aValue, MyEnum.min);
            var p = aValue.SetIntValue(-0.395);
            return aValue;
        }

        private static void FindFunc(ref LinkedListNode<char> nodeValue, out MyEnum funcValue)
        {
            funcValue = MyEnum.nun;
            var strFunc = "";
            while(nodeValue != null && !NumbersPredicate(nodeValue.Value) && nodeValue.Value != UnoMinus && nodeValue.Value != OpenBracket && nodeValue.Value != CharVariable)
            {
                strFunc += nodeValue.Value;
                nodeValue = nodeValue.Next;
            }

            switch (strFunc)
            {
                case "sin":
                {
                    funcValue = MyEnum.sin;
                    break;
                }
                case "cos":
                {
                    funcValue = MyEnum.cos;
                    break;
                }
                case "log":
                {
                    funcValue = MyEnum.log;
                    break;
                }
                default:
                {
                    if(strFunc.Length > 0) throw new Exception("Неверный ввод");
                    break;
                }
            }
        }

        //static bool OperandCompare(string op, string nextOp)
        //{
        //    int opNum = CharGroup0.Any(ch => ch == Convert.ToString(op)) ? 0
        //        : CharGroup1.Any(ch => ch == Convert.ToString(op)) ? 1 : 2; //TODO throw
        //    int nextOpNum = CharGroup0.Any(ch => ch == Convert.ToString(nextOp)) ? 0
        //        : CharGroup1.Any(ch => ch == Convert.ToString(nextOp)) ? 1 : 2;
        //    return opNum >= nextOpNum;
        //}

        static IOperand DuoOperand(IOperand a, IOperand b, char operand)
        {
            IOperand value;
            switch (operand)
            {
                case '+':
                    value = new Add(a, b);
                    break;
                case '-':
                    value = new Min(a, b);
                    break;
                case '*':
                    value = new Umn(a, b);
                    break;
                case '/':
                    value = new Del(a, b);
                    break;
                case '^':
                    value = new Up(a, b);
                    break;
                default:
                    throw new Exception("Неверная запись");
            }
            return value;
        }

        static IOperand UnoOperand(IOperand a, MyEnum operand)
        {
            IOperand value;
            switch (operand)
            {
                case MyEnum.nun:
                    value = a;
                    break;
                case MyEnum.sin:
                    value = new Sin(a);
                    break;
                case MyEnum.cos:
                    value = new Cos(a);
                    break;
                case MyEnum.log:
                    value = new Ln(a);
                    break;
                case MyEnum.min:
                    value = new UnoMin(a);
                    break;
                default:
                    throw new Exception("Неверная запись");
            }
            return value;
        }

        static bool NumbersPredicate(char ch) => (ch >= '0') && (ch <= '9') || (ch == '.');

        static string EnumToString(IEnumerable<char> enumChars)
        {
            var s = "";
            foreach (var enumChar in enumChars)
                s += enumChar != '.' ? enumChar : ',';
            return s;
        }
    }

    interface IOperand
    {
        double SetIntValue(double value);
    }

    class Const : IOperand
    {
        private readonly double _doubValue;

        public Const(double value)
        {
            _doubValue = value;
        }


        public double SetIntValue(double value)
        {
            //intValue = value;
            //throw new NotImplementedException();
            return _doubValue;
        }
    }

    class Variable : IOperand
    {
        private double _doubValue;

        public double SetIntValue(double value)
        {
            _doubValue = value;
            return _doubValue;
            //throw new NotImplementedException();
        }
    }

    abstract class UnoMethod : IOperand
    {
        private readonly IOperand _value1;
        private double _resultValue;

        protected UnoMethod(IOperand a)
        {
            _value1 = a;
        }

        protected abstract double Operation(double a);

        public double SetIntValue(double value)
        {
            _resultValue = Operation(_value1.SetIntValue(value));
            return _resultValue;
        }
    }

    class Sin : UnoMethod
    {
        public Sin(IOperand a) : base(a)
        {
        }

        protected override double Operation(double a)
        {
            return Math.Sin(a);
            //throw new NotImplementedException();
        }
    }

    class Cos : UnoMethod
    {
        public Cos(IOperand a) : base(a)
        {
        }

        protected override double Operation(double a)
        {
            return Math.Cos(a);
            //throw new NotImplementedException();
        }
    }

    class Ln : UnoMethod
    {
        public Ln(IOperand a) : base(a)
        {
        }

        protected override double Operation(double a)
        {
            return Math.Log(a);
            //throw new NotImplementedException();
        }
    }

    class UnoMin : UnoMethod
    {
        public UnoMin(IOperand a) : base(a)
        {
        }

        protected override double Operation(double a)
        {
            return -a;
            //throw new NotImplementedException();
        }
    }

    abstract class Method : IOperand
    {
        private readonly IOperand _value1;
        private readonly IOperand _value2;
        private double _resultValue;

        //private int varible;
        protected Method(IOperand a, IOperand b)
        {
            _value1 = a;
            _value2 = b;
            //Operation(a, b);
        }

        public abstract double Operation(double a, double b);

        public double SetIntValue(double value)
        {

            _resultValue = Operation(_value1.SetIntValue(value), _value2.SetIntValue(value));
            return _resultValue;
            //_value.SetIntValue(value);
            //throw new NotImplementedException();
        }
    }

    class Min : Method
    {
        public Min(IOperand a, IOperand b) : base(a, b)
        {
        }

        public override double Operation(double a, double b)
        {
            return (a - b);
            //throw new NotImplementedException();
        }
    }

    class Add : Method
    {
        public Add(IOperand a, IOperand b) : base(a, b)
        {
        }

        public override double Operation(double a, double b)
        {
            return a + b;
            //throw new NotImplementedException();
        }
    }

    class Umn : Method
    {
        public Umn(IOperand a, IOperand b) : base(a, b)
        {
        }

        public override double Operation(double a, double b)
        {
            return a * b;
            //throw new NotImplementedException();
        }
    }

    class Del : Method
    {
        public Del(IOperand a, IOperand b) : base(a, b)
        {
        }

        public override double Operation(double a, double b)
        {
            if (Math.Abs(b) < 0.000000000000000000000001) throw new Exception("не делите на ноль");
            return a / b;
            //throw new NotImplementedException();
        }
    }
    class Up : Method
    {
        public Up(IOperand a, IOperand b) : base(a, b)
        {
        }

        public override double Operation(double a, double b)
        {
            //if (Math.Abs(b) % 2 < 0.000000001 && a < 0) throw new Exception("отрицательный показатель");
            var c = Math.Pow(a, b);
            return Math.Pow(a, b);
            //throw new NotImplementedException();
        }
    }


}
