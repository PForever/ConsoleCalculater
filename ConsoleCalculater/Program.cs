using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var str = "-sin(-3log(- 3 sin -1.5157 x ^ 3.5x / -x ^ 25.27 log-x)(18x + 3.87)/x^-20)";
            double dbX = -0.395;
            try
            {
                Console.WriteLine($"x = {dbX}\n{str} = {Calc.Start(str, dbX)}");
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
        private enum FuncEnum {Nun, Sin, Cos, Log, Min};


        public static double Start(string startValue, double xValue)
        {
            var node = new LinkedList<char>(startValue.Where(ch => ch != ' ')).First;
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
                        a = DuoOperand(a, DuoOperand(b, StrResult(operand, ref sValue, out char tempOperand), nextOperand), operand);
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
            FindFunc(ref charter, out FuncEnum funcValue);
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
                    aValue = strNumbs == "" ? (IOperand) new Variable() : new Umn(new Const(Double.Parse(strNumbs.Replace('.', ','))), new Variable());
                    charter = charter.Next;
                }
                else
                {
                    if (strNumbs == "") throw new Exception("Некорректная запись");
                    aValue = new Const(strNumbs == "" ? 1 : Double.Parse(strNumbs.Replace('.', ',')));
                }
                if (blUno2) aValue = UnoOperand(aValue, FuncEnum.Min);
            }
            aValue = UnoOperand(aValue, funcValue);
            if (blUno1) aValue = UnoOperand(aValue, FuncEnum.Min);
            return aValue;
        }

        private static void FindFunc(ref LinkedListNode<char> nodeValue, out FuncEnum funcValue)
        {
            funcValue = FuncEnum.Nun;
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
                    funcValue = FuncEnum.Sin;
                    break;
                }
                case "cos":
                {
                    funcValue = FuncEnum.Cos;
                    break;
                }
                case "log":
                {
                    funcValue = FuncEnum.Log;
                    break;
                }
                default:
                {
                    if(strFunc.Length > 0) throw new Exception("Некорректная запись");
                    break;
                }
            }
        }


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
                    throw new Exception("Некорректная запись");
            }
            return value;
        }

        static IOperand UnoOperand(IOperand a, FuncEnum operand)
        {
            IOperand value;
            switch (operand)
            {
                case FuncEnum.Nun:
                    value = a;
                    break;
                case FuncEnum.Sin:
                    value = new Sin(a);
                    break;
                case FuncEnum.Cos:
                    value = new Cos(a);
                    break;
                case FuncEnum.Log:
                    value = new Ln(a);
                    break;
                case FuncEnum.Min:
                    value = new UnoMin(a);
                    break;
                default:
                    throw new Exception("Некорректная запись");
            }
            return value;
        }

        static bool NumbersPredicate(char ch) => (ch >= '0') && (ch <= '9') || (ch == '.');
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
        }
    }

    abstract class Method : IOperand
    {
        private readonly IOperand _value1;
        private readonly IOperand _value2;
        private double _resultValue;

        protected Method(IOperand a, IOperand b)
        {
            _value1 = a;
            _value2 = b;
        }

        public abstract double Operation(double a, double b);

        public double SetIntValue(double value)
        {
            _resultValue = Operation(_value1.SetIntValue(value), _value2.SetIntValue(value));
            return _resultValue;
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
        }
    }
    class Up : Method
    {
        public Up(IOperand a, IOperand b) : base(a, b)
        {
        }

        public override double Operation(double a, double b)
        {
            return Math.Pow(a, b);
        }
    }


}
