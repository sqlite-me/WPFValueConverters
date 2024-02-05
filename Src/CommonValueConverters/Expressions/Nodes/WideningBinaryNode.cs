namespace CommonValueConverters.Converters.Expressions.Nodes
{
    using HelperTrinity;
    using System;
    using System.Diagnostics;
    using System.Windows;
    using LExp = System.Linq.Expressions.Expression;

    // a binary node that automatically widens one value to match the width of the other if necessary
    internal abstract class WideningBinaryNode : BinaryNode
    {
        private static readonly ExceptionHelper exceptionHelper = new ExceptionHelper(typeof(WideningBinaryNode));

        protected WideningBinaryNode(Node leftNode, Node rightNode)
            : base(leftNode, rightNode)
        {
        }

        public sealed override object Evaluate(NodeEvaluationContext evaluationContext)
        {
            var leftNodeValue = LeftNode.Evaluate(evaluationContext);
            var rightNodeValue = RightNode.Evaluate(evaluationContext);

            if (leftNodeValue == DependencyProperty.UnsetValue || rightNodeValue == DependencyProperty.UnsetValue)
            {
                return DependencyProperty.UnsetValue;
            }

            var leftNodeValueType = GetNodeValueType(leftNodeValue);
            var rightNodeValueType = GetNodeValueType(rightNodeValue);

            // determine the type to which we will need to widen any narrower value
            var maxNodeValueType = (NodeValueType)Math.Max((int)leftNodeValueType, (int)rightNodeValueType);

            // we have to convert rather than just cast because the value is boxed
            var convertibleLeftNodeValue = leftNodeValue as IConvertible;
            var convertibleRightNodeValue = rightNodeValue as IConvertible;
            Debug.Assert(convertibleLeftNodeValue != null || (maxNodeValueType == NodeValueType.String || maxNodeValueType == NodeValueType.ReferenceType));
            Debug.Assert(convertibleRightNodeValue != null || (maxNodeValueType == NodeValueType.String || maxNodeValueType == NodeValueType.ReferenceType));

            var succeeded = false;
            object result = null;

            switch (maxNodeValueType)
            {
                case NodeValueType.String:
                    succeeded = this.DoString(convertibleLeftNodeValue == null ? null : convertibleLeftNodeValue.ToString(null), convertibleRightNodeValue == null ? null : convertibleRightNodeValue.ToString(null), out result);
                    break;
                case NodeValueType.Boolean:
                    succeeded = this.DoBoolean(convertibleLeftNodeValue.ToBoolean(null), convertibleRightNodeValue.ToBoolean(null), out result);
                    break;
                case NodeValueType.Byte:
                    succeeded = this.DoByte(convertibleLeftNodeValue.ToByte(null), convertibleRightNodeValue.ToByte(null), out result);
                    break;
                case NodeValueType.Int16:
                    succeeded = this.DoInt16(convertibleLeftNodeValue.ToInt16(null), convertibleRightNodeValue.ToInt16(null), out result);
                    break;
                case NodeValueType.Int32:
                    succeeded = this.DoInt32(convertibleLeftNodeValue.ToInt32(null), convertibleRightNodeValue.ToInt32(null), out result);
                    break;
                case NodeValueType.Int64:
                    succeeded = this.DoInt64(convertibleLeftNodeValue.ToInt64(null), convertibleRightNodeValue.ToInt64(null), out result);
                    break;
                case NodeValueType.Single:
                    succeeded = this.DoSingle(convertibleLeftNodeValue.ToSingle(null), convertibleRightNodeValue.ToSingle(null), out result);
                    break;
                case NodeValueType.Double:
                    succeeded = this.DoDouble(convertibleLeftNodeValue.ToDouble(null), convertibleRightNodeValue.ToDouble(null), out result);
                    break;
                case NodeValueType.Decimal:
                    succeeded = this.DoDecimal(convertibleLeftNodeValue.ToDecimal(null), convertibleRightNodeValue.ToDecimal(null), out result);
                    break;
                case NodeValueType.ValueType:
                    succeeded = this.DoValueType(leftNodeValue, rightNodeValue, out result);
                    break;
                case NodeValueType.ReferenceType:
                    succeeded = this.DoReferenceType(leftNodeValue, rightNodeValue, out result);
                    break;
            }

            exceptionHelper.ResolveAndThrowIf(!succeeded, "OperatorNotSupportedWithOperands", this.OperatorSymbols, leftNodeValueType, rightNodeValueType);
            return result;
        }

        protected virtual bool DoString(string value1, string value2, out object result)
        {
            result = null;
            return false;
        }

        protected virtual bool DoBoolean(bool value1, bool value2, out object result)
        {
            result = null;
            return false;
        }

        protected virtual bool DoByte(byte value1, byte value2, out object result)
        {
            result = null;
            return false;
        }

        protected virtual bool DoInt16(short value1, short value2, out object result)
        {
            result = null;
            return false;
        }

        protected virtual bool DoInt32(int value1, int value2, out object result)
        {
            result = null;
            return false;
        }

        protected virtual bool DoInt64(long value1, long value2, out object result)
        {
            result = null;
            return false;
        }

        protected virtual bool DoSingle(float value1, float value2, out object result)
        {
            result = null;
            return false;
        }

        protected virtual bool DoDouble(double value1, double value2, out object result)
        {
            result = null;
            return false;
        }

        protected virtual bool DoDecimal(decimal value1, decimal value2, out object result)
        {
            result = null;
            return false;
        }

        protected virtual bool DoValueType(object value1, object value2, out object result)
        {
            return doDefault(value1, value2, out result);
        }

        protected virtual bool DoReferenceType(object value1, object value2, out object result)
        {
            return doDefault(value1, value2, out result);
        }

        private bool doDefault(object value1, object value2, out object? result)
        {
            var l = LExp.Constant(value1);
            var r = LExp.Constant(value2);

            switch (OperatorSymbols)
            {
                case "==":
                    case"!=":
                case ">":
                    case "<":
                case ">=":
                case "<=":
                    break;
                        default:
                    if(value1== null||value2==null||value1 == DependencyProperty.UnsetValue || value2 == DependencyProperty.UnsetValue)
                    {
                        result = DependencyProperty.UnsetValue;
                        return true;
                    }
                    break;
            }

            LExp? expression = OperatorSymbols switch
            {
                "+" => LExp.Add(l, r),
                "-" => LExp.Subtract(l, r),
                "*" => LExp.Multiply(l, r),
                "/" => LExp.Divide(l, r),
                "%" => LExp.Modulo(l, r),
                "==" => LExp.Equal(l, r),
                ">" => LExp.GreaterThan(l, r),
                "<" => LExp.LessThan(l, r),
                "!=" => LExp.NotEqual(l, r),
                ">=" => LExp.GreaterThanOrEqual(l, r),
                "<=" => LExp.LessThanOrEqual(l, r),
                ">>" => LExp.RightShift(l, r),
                "<<" => LExp.LeftShift(l, r),
                "||" => LExp.OrElse(l, r),
                "&&" => LExp.AndAlso(l, r),
                "|" => LExp.Or(l, r),
                "&" => LExp.And(l, r),
                "+=" => LExp.AddAssign(l, r),
                "-=" => LExp.SubtractAssign(l, r),
                "*=" => LExp.MultiplyAssign(l, r),
                "/=" => LExp.DivideAssign(l, r),
                "%=" => LExp.MultiplyAssign(l, r),
                _ => null,
            };
            if (expression != null) {
                result = LExp.Lambda(expression).Compile().DynamicInvoke();
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}
