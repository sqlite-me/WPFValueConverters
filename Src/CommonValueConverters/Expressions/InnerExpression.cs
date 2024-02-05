using CommonValueConverters.Converters.Expressions.Nodes;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CommonValueConverters.Expressions
{
    internal class InnerExpression
    {
        /// <summary>
        /// 匹配 字符串、true、false、null、is、is not、as、.[Property]、{\d}
        /// </summary>
        private static Regex regex = new Regex("(?<!\\\\)\".*?(?<!\\\\)\"|true|false|null|is\\s+not|is|as|\\?\\?|\\?|(\\.\\s*[_|a-z|A-Z]\\w*)|{-?\\d+}", RegexOptions.IgnoreCase);
        private static Regex constNumberRegex = new Regex("((\\d+\\.?\\d*)|(\\d*\\.\\d+))((d|f|m|ul|lu|l|u)?)", RegexOptions.IgnoreCase);
        private List<Match> matchsList;
        private List<Match> matchsNumList;
        private NodeBracket root;

        public InnerExpression(string str)
        {
            matchsList= regex.Matches(str).ToList();
            matchsNumList= new List<Match>();
            foreach (Match one in constNumberRegex.Matches(str))
            {
                var end = one.Index + one.Length-1;
                var has= matchsList.Any(t =>
                {
                    var tEnd = t.Index + t.Length - 1;
                    return (one.Index >= t.Index && one.Index <= tEnd) || (end >= t.Index && end <= tEnd);
                });
                if (has) { continue; }
                matchsNumList.Add(one);
            }
            if(matchsNumList.Count > 0)
            {
                matchsList.AddRange(matchsNumList);
            }
            root=new();
            parseBrackets(str);
        }
        private void parseBrackets(string str)
        {
            var opretorChars = new char[] { '=', '>', '<', '|', '&', '^', '+', '-', '*', '/', '%' };
            var dicStrs = matchsList.ToDictionary(t => t.Index, t => t);
            Stack<NodeBracket> bracketSStack = new Stack<NodeBracket>();
            bracketSStack.Push(root);
            var stackTop = root;
            for (var i = 0; i < str.Length; i++)
            {
                if (dicStrs.ContainsKey(i))
                {
                    var match = dicStrs[i];
                    var upVal = match.Value.ToUpper();
                    switch (upVal)
                    {
                        case "TRUE":
                            stackTop.AddNode(new NodeConst(true));
                            break;
                        case "FALSE":
                            stackTop.AddNode(new NodeConst(false));
                            break;
                        case "NULL":
                            stackTop.AddNode(new NodeConst(null));
                            break;
                        case "IS":
                        case "AS":
                           var type = parseType(str, match.Index + match.Length,out int len);
                            i += len;
                            stackTop.AddNode(new NodeOperatorType(upVal, type));
                            break;
                        case "?":
                            stackTop.AddNode(new NodeOperatorTernary("?"));
                            break;
                        case "??":
                            stackTop.AddNode(new NodeOperatorDouble("??"));
                            break;
                        default:
                            if(upVal.StartsWith("IS ")&&upVal.Replace(" ", "") == "ISNOT")// is not
                            {
                                 type = parseType(str, match.Index + match.Length, out len);
                                i += len;
                                stackTop.AddNode(new NodeOperatorType(upVal, type));
                            }
                            else if (upVal[0]=='\"')//string
                            {
                                stackTop.AddNode(new NodeConst(match.Value.Trim('\"')));
                            }
                            else if (upVal[0] == '{')//param
                            {
                                var index = int.Parse(upVal.Substring(1, upVal.Length - 2).Trim());
                                stackTop.AddNode(new NodeParam(index));
                            }
                            else if (upVal[0] == '.' && (upVal[1]==' '|| upVal[1]=='_'|| (upVal[1]>='a'&& upVal[1]<='z')||(upVal[1]>='A'&& upVal[1] <= 'Z')))
                            {//.Property
                                stackTop.AddNode(new NodeOperatorAccess(match.Value.Trim(' ','.')));
                            }
                            else//num
                            {
                                stackTop.AddNode(new NodeConst(stringToNumConst(match.ValueSpan)));
                            }
                            break;
                    }
                    i += match.Length-1;
                    continue;
                }

                switch (str[i])
                {
                    case ' ':
                        continue;
                    case '(':
                    bracketSStack.Push(stackTop = new NodeBracket { Start = i });
                        break;
                        case ')':
                        var current = bracketSStack.Pop();
                        current.Length = i - current.Start;
                        if (bracketSStack.Count > 0)
                        {
                            stackTop = bracketSStack.Pop();
                            bracketSStack.Push(stackTop);
                            stackTop.AddBracketNode(current);
                        }
                        else
                        {
                            throw new ApplicationException("error");
                        }
                        break;
                    case '!':
                        if (str[i + 1] == '=')
                        {
                            stackTop.AddNode(new NodeOperatorDouble("!="));
                            i++;
                        }
                        else
                            stackTop.AddNode(new NodeOperatorSingle("!"));
                        break;
                        default:
                        if (opretorChars.Contains(str[i + 1]))
                        {
                            stackTop.AddNode(new NodeOperatorDouble(new string(new[] { str[i], str[i+1] })));
                            i++;
                        }else
                        stackTop.AddNode(new NodeOperatorDouble(new string(new[] { str[i] })));
                        break;
                }
            }
        }

        private Type? parseType(string str,int i,out int len)
        {
            var tmp =str.Substring(i);
            var match = Regex.Match(tmp, @"^\s*[\w|_][\w|_|\d|\.|<|>]*");
            var strValue = match.Value.Trim();
            var match1 = matchsList.FirstOrDefault(t => t.Value == strValue);
            if(match1 != null)
            {
                matchsList.Remove(match1);
            }
            var type = Type.GetType(strValue);
            len = match.Length;
            return type;
        }

        private object stringToNumConst(ReadOnlySpan<char> value)
        {
            char? last = value.Length>0? value[value.Length - 1]:null;
            char? lastSecond = value.Length > 1 ? value[value.Length - 2] : null;
            string type;
            ReadOnlySpan<char> val;
            if (lastSecond.HasValue && ((lastSecond > 'a' && lastSecond < 'z') || (lastSecond > 'A' && lastSecond < 'Z')))
            {
                val=value.Slice(0,value.Length - 2);
                type =new string(new[] {lastSecond.Value,last.Value }).ToUpper();
            }else if(last.HasValue&& ((last > 'a' && last < 'z') || (last > 'A' && last < 'Z')))
            {
                val = value.Slice(0, value.Length - 1);
                type = new string(new[] { last.Value }).ToUpper();
            }else if (value.Contains('.'))
            {
                val = value;
                type = "D";
            }
            else
            {
                val = value;
                type = "";
            }
            switch(type)
            {
                case "D":
                    return double.Parse(val);
                case "F": 
                    return float.Parse(val);
                case "M": 
                    return decimal.Parse(val);
                case "UL": 
                case "LU":
                    return ulong.Parse(val);
                case "L":
                    return long.Parse(val);
                case "U":
                    return uint.Parse(val);
                default:
                    return int.Parse(val);
            }
        }

        private void parseExp(string str,int start,int len)
        {

        }

        private abstract class NodeData
        {
            public NodeData(NodeType nodeType)
            {
                this.NodeType = nodeType;
            }
            public NodeType NodeType { get; }
            public NodeData? Parent { get; set; }
        }
        private class NodeBracket: NodeData
        {
            public NodeBracket():base(NodeType.Bracket) { }
            public int Start { get; set; }
            public int Length { get; set; }
            public List<NodeBracket> BracketNodes { get; }= new List<NodeBracket>();
            public bool Closed => Length > 0;
            public List<NodeData> NodeDatas { get; }=new List<NodeData>();
            public void AddNode(NodeData nodeData)
            {
                nodeData.Parent= this;
                NodeDatas.Add(nodeData);
            }
            public void AddBracketNode(NodeBracket nodeBracket)
            {
                AddNode(nodeBracket);
                BracketNodes.Add(nodeBracket);
            }
        }

        private class NodeConst : NodeData
        {
            public NodeConst(object? value) : base(NodeType.Const)
            {
                ConstantExpression= Expression.Constant(value);
            }

            public ConstantExpression ConstantExpression { get; }
        }
        private class NodeParam : NodeData
        {
            public NodeParam(int index) : base(NodeType.Param)
            {
                ParamIndex = index;
            }

            public int ParamIndex { get;}
        }
        private abstract class NodeOperator : NodeData
        {
            public NodeOperator(NodeType nodeType, string @operator) : base(nodeType)
            {
                this.Operator = @operator;
            }

            public string Operator { get; }
        }
        private class NodeOperatorSingle : NodeOperator
        {
            public NodeOperatorSingle(string @operator) : base(NodeType.Operator_Single,@operator)
            {
            }

            public NodeData Target { get; }
        }
        private class NodeOperatorDouble : NodeOperator
        {
            public NodeOperatorDouble(string @operator) : base(NodeType.Operator_Single, @operator)
            {
            }
            public NodeData Left { get; }
            public NodeData Right { get; protected set; }
        }
        private class NodeOperatorType : NodeOperator
        {
            public NodeOperatorType(string @operator,Type type) : base(NodeType.Operator_Type, @operator)
            {
                this.Type= type;
            }
            public NodeData Left { get; }
            public Type Type { get; }
        }
        private class NodeOperatorTernary : NodeOperator
        {
            public NodeOperatorTernary(string @operator) : base(NodeType.Operator_Ternary, @operator)
            {
            }


            public NodeData First { get; }
            public NodeData Sec0nd { get; }
            public NodeData Third { get; }

        }
        private class NodeOperatorAccess : NodeOperator
        {
            public NodeOperatorAccess(string propertyName) : base(NodeType.Operator_Access, ".")
            {this.PropertyName = propertyName;
            }

            public NodeData Target { get; }

            public string PropertyName { get; }
        }
        private enum NodeType
        {
            Const,
            Param,
            /// <summary>
            /// !
            /// </summary>
            Operator_Single,
            /// <summary>
            /// + - * / % == !== > &lt; >= &lt;= | &amp; &lt;&lt; >> ^ ?? 
            /// </summary>
            Operator_Double,
            /// <summary>
            /// ? :
            /// </summary>
            Operator_Ternary,
            /// <summary>
            /// .[ProName]
            /// </summary>
            Operator_Access,
            /// <summary>
            /// is (is not) as
            /// </summary>
            Operator_Type,
            /// <summary>
            /// ()
            /// </summary>
            Bracket,
        }
        public static void Test(object[] values)
        {
            var c1 = Expression.Constant(values[0]);
            var c2 = Expression.Constant(values[1]);
            var tmp = Expression.Subtract(c1, c2);
            var tmm = Expression.Lambda(tmp).Compile().DynamicInvoke();
            //Expression.Property()
            //Expression.Dynamic()
            //Expression.Lambda()
            var pa = Expression.Parameter(typeof(DateTime), "a");
            var pb = Expression.Parameter(typeof(DateTime), "b");
            var tmpP = Expression.Subtract(pa, pb);

            var tmm2 = Expression.Lambda(tmpP, pa, pb).Compile().DynamicInvoke(new[] { values[0], values[1] });
            
        }
    }
}
