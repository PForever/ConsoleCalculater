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



            //s.Where(value =>
            //{
            //    var flag = charLst.Any(charValue => charValue == value);
            //        if (flag)
            //        {
            //            valueList[valueList.Count - 1] += value;
            //        }
            //        return charLst.Any(charValue => charValue == value);
            //});



            //foreach (var variable in s)
            //{
            //    if s ==
            //}
            //var p = Double.Parse(Console.ReadLine());
            var str = "- sin2^-2";//"- 1 * - sin - 4 ^ - 2";
            Console.WriteLine($"{str} = {Calc.Start(str)}");
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
        private static readonly char[] CharList = new char[] { '*', '/', '+', '-', '^', '(', ')' };
        private static readonly string[] CharGroup0 = new string[] { "+", "-" };
        private static readonly string[] CharGroup1 = new string[] { "*", "/" };
        private static readonly string[] CharGroup2 = new string[] { "^" };

        public static string Start(string startValue) => EnumToString(StrResult(EnumToString(startValue.Where(ch => ch != ' '))));

        //private static string StrResult(string source)
        //{
        //    var a = source.TakeWhile()
        //}

        //private static string FindA(string source)
        //{

        //}
        private static IEnumerable<char> StrResult(IEnumerable<char> sValue)
        {
            var s = EnumToString(sValue);
            var a = sValue.TakeWhile(NumbersPredicate);
            var intSkipInterval = a.Count();
            var operand = sValue.Skip(intSkipInterval).TakeWhile(ch => !NumbersPredicate(ch));
            intSkipInterval += operand.Count();
            while (intSkipInterval < sValue.Count())
            {
                var boolUno = false;
                var b = sValue.Skip(intSkipInterval).TakeWhile(NumbersPredicate);
                intSkipInterval += b.Count();

                //if (operand.Count() > 1 && CharList.Any(ch => ch == operand.First())) //TODO -1
                //{
                //    var op = EnumToString(operand);
                //    if (operand.Count() > 2)
                //    {
                //        if (operand.Last() == '-')
                //        {
                //            operand = operand.Take(operand.Count() - 1);
                //            b = UnoOperand(EnumToString(b), "-");
                //        }
                //        if (operand.Skip(1).First() == '-')
                //        {
                //            //b = UnoOperand(EnumToString(b), "-");
                //            b = UnoOperand(EnumToString(UnoOperand(EnumToString(b), EnumToString(operand.Skip(2)))), "-");
                //        }
                //        else b = UnoOperand(EnumToString(b), EnumToString(operand.Skip(1)));
                //    }
                //    else b = UnoOperand(EnumToString(b), EnumToString(operand.Skip(1)));
                //    operand = operand.Take(1);
                //}
                //else if (operand.Count() > 1 || operand.First() == '-')
                //{
                //    if (operand.Count() > 1 && operand.Last() == '-')
                //    {
                //        operand = operand.Take(operand.Count() - 1);
                //        b = UnoOperand(EnumToString(b), "-");
                //    }
                //    b = UnoOperand(EnumToString(b), EnumToString(operand));
                //    var sb = EnumToString(b);
                //    if (!a.Any()) boolUno = true;
                //    else if (operand.First() != '-')
                //    {
                //        operand = "*";
                //    }
                //    else operand = "+";
                //}

                var nextOperand = sValue.Skip(intSkipInterval).TakeWhile(ch => !NumbersPredicate(ch));
                var t = EnumToString(nextOperand);
                intSkipInterval += nextOperand.Count();


                //if (boolUno)
                //{
                //    a = b;
                //    operand = nextOperand;
                //}
                //else if (nextOperand.Count() > 2 && OperandCompare(EnumToString(operand), "*") || OperandCompare(EnumToString(operand), EnumToString(nextOperand.Take(1))))
                //{
                //    var op = EnumToString(operand);
                //    a = DuoOperand(EnumToString(a), EnumToString(b), EnumToString(operand));
                //    operand = nextOperand;
                //}
                //else
                //{
                //    var sa = EnumToString(a);
                //    a = DuoOperand(EnumToString(a), EnumToString(StrResult(b.Concat(nextOperand.Concat(sValue.Skip(intSkipInterval))))), EnumToString(operand));
                //    break;
                //}
            }
            return a;
        }

        private static void Find(string source)
        {

        }

        static bool OperandCompare(string op, string nextOp)
        {
            int opNum = CharGroup0.Any(ch => ch == Convert.ToString(op)) ? 0
                : CharGroup1.Any(ch => ch == Convert.ToString(op)) ? 1 : 2; //TODO throw
            int nextOpNum = CharGroup0.Any(ch => ch == Convert.ToString(nextOp)) ? 0
                : CharGroup1.Any(ch => ch == Convert.ToString(nextOp)) ? 1 : 2;
            return opNum >= nextOpNum;
        }

        static IEnumerable<char> DuoOperand(string a, string b, string operand)
        {
            IOperand value;
            switch (operand)
            {
                case "+":
                    value = new Add(new Const(Convert.ToDouble(a)), new Const(Double.Parse(b)));
                    break;
                case "-":
                    value = new Min(new Const(Convert.ToDouble(a)), new Const(Double.Parse(b)));
                    break;
                case "*":
                    value = new Umn(new Const(Convert.ToDouble(a)), new Const(Double.Parse(b)));
                    break;
                case "/":
                    value = new Del(new Const(Convert.ToDouble(a)), new Const(Double.Parse(b)));
                    break;
                case "^":
                    value = new Up(new Const(Convert.ToDouble(a)), new Const(Double.Parse(b)));
                    break;
                default:
                    throw new Exception("Неверная запись");
            }
            return value.SetIntValue(0).ToString(CultureInfo.InvariantCulture);
        }

        static IEnumerable<char> UnoOperand(string a, string operand)
        {
            IOperand value;
            switch (operand)
            {
                case "sin":
                    value = new Sin(new Const(Double.Parse(a)));
                    break;
                case "cos":
                    value = new Cos(new Const(Double.Parse(a)));
                    break;
                case "ln":
                    value = new Ln(new Const(Double.Parse(a)));
                    break;
                case "-":
                    value = new UnoMin(new Const(Double.Parse(a)));
                    break;
                default:
                    throw new Exception("Неверная запись");
            }
            return value.SetIntValue(0).ToString(CultureInfo.InvariantCulture);
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
            if (Math.Abs(b) < 0.000000001) throw new Exception("не делите на ноль");
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
