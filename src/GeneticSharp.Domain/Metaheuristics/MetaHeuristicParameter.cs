using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{

    public delegate TParamType ParameterGenerator<out TParamType>(IMetaHeuristic h, IMetaHeuristicContext ctx);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1>(IMetaHeuristic h, IMetaHeuristicContext ctx, TArg1 arg1);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1, in TArg2>(IMetaHeuristic h, IMetaHeuristicContext ctx, TArg1 arg1, TArg2 arg2);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1, in TArg2, in TArg3>(IMetaHeuristic h, IMetaHeuristicContext ctx, TArg1 arg1, TArg2 arg2, TArg3 arg3);

    public interface IMetaHeuristicParameter
    {
         ParameterScope Scope { get; set; }

        TItemType GetOrAdd<TItemType>(IMetaHeuristic h, IMetaHeuristicContext ctx, string key);
        
        object ComputeParameter(IMetaHeuristic h, IMetaHeuristicContext ctx);


    }

    public interface IMetaHeuristicParameterGenerator<TParamType>: IMetaHeuristicParameter
    {

        ParameterGenerator<TParamType> GetGenerator(IMetaHeuristicContext ctx);

    }


    public interface IExpressionGeneratorParameter: IMetaHeuristicParameter
    {

        //string GetFormatKey();

        LambdaExpression GetExpression(IMetaHeuristicContext ctx, string paramName);

    }


    public class MetaHeuristicParameter<TParamType> : NamedEntity, IMetaHeuristicParameterGenerator<TParamType>
    {

       

        public ParameterScope Scope { get; set; }

        //private static readonly Dictionary<ParameterScope, string> _formatKeys = new Dictionary<ParameterScope, string>();


        //public static string GetFormatKey(ParameterScope mScope)
        //{
        //        var sb = new StringBuilder("{0}");
        //        if ((mScope & ParameterScope.Generation) == ParameterScope.Generation)
        //        {
        //            sb.Append("G{1}");

        //        }
        //        if ((mScope & ParameterScope.Stage) == ParameterScope.Stage)
        //        {
        //            sb.Append("S{2}");
        //        }
        //        if ((mScope & ParameterScope.MetaHeuristic) == ParameterScope.MetaHeuristic)
        //        {
        //            sb.Append("H{3}");
        //        }
        //        if ((mScope & ParameterScope.Individual) == ParameterScope.Individual)
        //        {
        //            sb.Append("I{4}");
        //        }

        //        return sb.ToString();
        //}


        //public string GetFormatKey()
        //{
           
        //    _formatKeys.TryGetValue(Scope, out var toReturn);
        //    if (string.IsNullOrEmpty(toReturn))
        //    {
        //        toReturn = GetFormatKey(Scope);
        //        lock (_formatKeys)
        //        {
        //            _formatKeys[Scope] = toReturn;
        //        }
        //    }

        //    return toReturn;
        //}

        //private string _formatKey;

        //public string GetKey(IMetaHeuristic h, IMetaHeuristicContext ctx, string key)
        //{
        //    if (_formatKey == null)
        //    {
        //        _formatKey = GetFormatKey();
        //    }

        //    return GetKey(h, ctx, _formatKey, key);

        //}

        //public static string GetKey(IMetaHeuristic h, IMetaHeuristicContext ctx, string formatKey, string key)
        //{
        //    return string.Format(formatKey,
        //        key,
        //        ctx.Population.GenerationsNumber.ToStringLookup(),
        //       ((int) ctx.CurrentStage).ToStringLookup(),
        //        h.Guid,
        //        ctx.Index.ToStringLookup());
        //}

        private (string key, int generation, MetaHeuristicsStage stage, IMetaHeuristic heuristic, int individual) GetScopeMask((string key, int generation, MetaHeuristicsStage stage, IMetaHeuristic heuristic, int individual) input)
        {
            if ((Scope & ParameterScope.Generation) != ParameterScope.Generation)
            {
                input.generation = 0;

            }
            if ((Scope & ParameterScope.Stage) != ParameterScope.Stage)
            {
                input.stage = MetaHeuristicsStage.All;
            }
            if ((Scope & ParameterScope.MetaHeuristic) != ParameterScope.MetaHeuristic)
            {
                input.heuristic = null;
            }
            if ((Scope & ParameterScope.Individual) != ParameterScope.Individual)
            {
                input.individual = 0;
            }

            return input;
        }

        public TItemType GetOrAdd<TItemType>(IMetaHeuristic h, IMetaHeuristicContext ctx, string key)
        {

            //var newKey = this.GetKey(h, ctx, key);
            var maskedTuple = GetScopeMask((key, ctx.Population.GenerationsNumber, ctx.CurrentStage, h, ctx.Index));

            return (TItemType)ctx.GetOrAdd(maskedTuple, () => (object)ComputeParameter(h, ctx));
        }



       public  object ComputeParameter(IMetaHeuristic h, IMetaHeuristicContext ctx)
        {
            return GetGenerator(ctx)(h, ctx);
        }

        public virtual ParameterGenerator<TParamType> GetGenerator(IMetaHeuristicContext ctx)
        {
            return Generator;
        }


        public ParameterGenerator<TParamType> Generator { get; set; }

        public TParamType GetOrAdd(IMetaHeuristic h, IMetaHeuristicContext ctx, string key)
        {
            return GetOrAdd<TParamType>(h, ctx, key);
        }
    }

   


    public class ExpressionMetaHeuristicParameter<TParamType> : MetaHeuristicParameter<TParamType>, IExpressionGeneratorParameter
    {

        private static Dictionary<Type, MethodInfo> _getOrAddMethods = new Dictionary<Type, MethodInfo>();

        public override ParameterGenerator<TParamType> GetGenerator(IMetaHeuristicContext ctx)
        {
            if (Generator == null)
            {
                Generator = GetDynamicGenerator(ctx).Compile();
            }

            return base.GetGenerator(ctx);
        }


        public virtual Expression<ParameterGenerator<TParamType>> GetDynamicGenerator(IMetaHeuristicContext ctx)
        {
            return DynamicGenerator;
        }


        public Expression<ParameterGenerator<TParamType>> DynamicGenerator { get; set; }

        public static MethodInfo GetOrAddMethod
        {
            get
            {
                if (!_getOrAddMethods.TryGetValue(typeof(TParamType), out var toReturn))
                {
                    var methods =
                        typeof(MetaHeuristicParameter<TParamType>).GetMethods();

                    toReturn = methods.First(m => m.Name == nameof(MetaHeuristicParameter<TParamType>.GetOrAdd) && !m.IsGenericMethod);
                    lock (_getOrAddMethods)
                    {
                        _getOrAddMethods[typeof(TParamType)] = toReturn;
                    }
                }
                return toReturn;
            }
        }

        public LambdaExpression GetExpression(IMetaHeuristicContext ctx, string paramName)
        {

            if (Scope == ParameterScope.None)
            {
                return GetDynamicGenerator(ctx);

            }
            else
            {
                var unCached = GetDynamicGenerator(ctx);

                try
                {
                    LambdaExpression cachedExpression = Expression.Lambda(Expression.Call(Expression.Constant(this), GetOrAddMethod, unCached.Parameters[0],
                            unCached.Parameters[1], Expression.Constant(paramName)), unCached.Parameters[0],
                        unCached.Parameters[1]);
                    return cachedExpression;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }

        }
    }

    public abstract class ExpressionMetaHeuristicParameterWithArgs<TParamType> : ExpressionMetaHeuristicParameter<TParamType>
    {

        public override Expression<ParameterGenerator<TParamType>> GetDynamicGenerator(IMetaHeuristicContext ctx)
        {
            if (DynamicGenerator == null)
            {
                DynamicGenerator = ParameterReplacer.ReduceLambdaParameterGenerator<TParamType>(GetExpressionWithArgs(), ctx);
            }
            return base.GetDynamicGenerator(ctx);
        }

        protected abstract LambdaExpression GetExpressionWithArgs();

    }


    public class ExpressionMetaHeuristicParameter<TParamType, TArg1> : ExpressionMetaHeuristicParameterWithArgs<TParamType>
    {
        protected override LambdaExpression GetExpressionWithArgs()
        {
            return DynamicGeneratorWithArg;
        }

        public Expression<ParameterGenerator<TParamType, TArg1>> DynamicGeneratorWithArg { get; set; }

    }


    public class ExpressionMetaHeuristicParameter<TParamType, TArg1, TArg2> : ExpressionMetaHeuristicParameterWithArgs<TParamType>
    {
        protected override LambdaExpression GetExpressionWithArgs()
        {
            return DynamicGeneratorWithArgs;
        }

        public Expression<ParameterGenerator<TParamType, TArg1, TArg2>> DynamicGeneratorWithArgs { get; set; }

    }

    public class ExpressionMetaHeuristicParameter<TParamType, TArg1, TArg2, TArg3> : ExpressionMetaHeuristicParameterWithArgs<TParamType>
    {
        protected override LambdaExpression GetExpressionWithArgs()
        {
            return DynamicGeneratorWithArgs;
        }

        public Expression<ParameterGenerator<TParamType, TArg1, TArg2, TArg3>> DynamicGeneratorWithArgs { get; set; }

    }

    static class ToStringExtensions
    {
        // Lookup table.
        static string[] _cache =
        {
        "0",
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        "10",
        "11",
        "12",
        "13",
        "14",
        "15",
        "16",
        "17",
        "18",
        "19",
        "20",
        "21",
        "22",
        "23",
        "24",
        "25",
        "26",
        "27",
        "28",
        "29",
        "30",
        "31",
        "32",
        "33",
        "34",
        "35",
        "36",
        "37",
        "38",
        "39",
        "40",
        "41",
        "42",
        "43",
        "44",
        "45",
        "46",
        "47",
        "48",
        "49",
        "50",
        "51",
        "52",
        "53",
        "54",
        "55",
        "56",
        "57",
        "58",
        "59",
        "60",
        "61",
        "62",
        "63",
        "64",
        "65",
        "66",
        "67",
        "68",
        "69",
        "70",
        "71",
        "72",
        "73",
        "74",
        "75",
        "76",
        "77",
        "78",
        "79",
        "80",
        "81",
        "82",
        "83",
        "84",
        "85",
        "86",
        "87",
        "88",
        "89",
        "90",
        "91",
        "92",
        "93",
        "94",
        "95",
        "96",
        "97",
        "98",
        "99",
        "100",
        "101",
        "102",
        "103",
        "104",
        "105",
        "106",
        "107",
        "108",
        "109",
        "110",
        "111",
        "112",
        "113",
        "114",
        "115",
        "116",
        "117",
        "118",
        "119",
        "120",
        "121",
        "122",
        "123",
        "124",
        "125",
        "126",
        "127",
        "128",
        "129",
        "130",
        "131",
        "132",
        "133",
        "134",
        "135",
        "136",
        "137",
        "138",
        "139",
        "140",
        "141",
        "142",
        "143",
        "144",
        "145",
        "146",
        "147",
        "148",
        "149",
        "150",
        "151",
        "152",
        "153",
        "154",
        "155",
        "156",
        "157",
        "158",
        "159",
        "160",
        "161",
        "162",
        "163",
        "164",
        "165",
        "166",
        "167",
        "168",
        "169",
        "170",
        "171",
        "172",
        "173",
        "174",
        "175",
        "176",
        "177",
        "178",
        "179",
        "180",
        "181",
        "182",
        "183",
        "184",
        "185",
        "186",
        "187",
        "188",
        "189",
        "190",
        "191",
        "192",
        "193",
        "194",
        "195",
        "196",
        "197",
        "198",
        "199",
        "200",
        "201",
        "202",
        "203",
        "204",
        "205",
        "206",
        "207",
        "208",
        "209",
        "210",
        "211",
        "212",
        "213",
        "214",
        "215",
        "216",
        "217",
        "218",
        "219",
        "220",
        "221",
        "222",
        "223",
        "224",
        "225",
        "226",
        "227",
        "228",
        "229",
        "230",
        "231",
        "232",
        "233",
        "234",
        "235",
        "236",
        "237",
        "238",
        "239",
        "240",
        "241",
        "242",
        "243",
        "244",
        "245",
        "246",
        "247",
        "248",
        "249",
        "250",
        "251",
        "252",
        "253",
        "254",
        "255",
        "256",
        "257",
        "258",
        "259",
        "260",
        "261",
        "262",
        "263",
        "264",
        "265",
        "266",
        "267",
        "268",
        "269",
        "270",
        "271",
        "272",
        "273",
        "274",
        "275",
        "276",
        "277",
        "278",
        "279",
        "280",
        "281",
        "282",
        "283",
        "284",
        "285",
        "286",
        "287",
        "288",
        "289",
        "290",
        "291",
        "292",
        "293",
        "294",
        "295",
        "296",
        "297",
        "298",
        "299",
        "300",
        "301",
        "302",
        "303",
        "304",
        "305",
        "306",
        "307",
        "308",
        "309",
        "310",
        "311",
        "312",
        "313",
        "314",
        "315",
        "316",
        "317",
        "318",
        "319",
        "320",
        "321",
        "322",
        "323",
        "324",
        "325",
        "326",
        "327",
        "328",
        "329",
        "330",
        "331",
        "332",
        "333",
        "334",
        "335",
        "336",
        "337",
        "338",
        "339",
        "340",
        "341",
        "342",
        "343",
        "344",
        "345",
        "346",
        "347",
        "348",
        "349",
        "350",
        "351",
        "352",
        "353",
        "354",
        "355",
        "356",
        "357",
        "358",
        "359",
        "360",
        "361",
        "362",
        "363",
        "364",
        "365",
        "366",
        "367",
        "368",
        "369",
        "370",
        "371",
        "372",
        "373",
        "374",
        "375",
        "376",
        "377",
        "378",
        "379",
        "380",
        "381",
        "382",
        "383",
        "384",
        "385",
        "386",
        "387",
        "388",
        "389",
        "390",
        "391",
        "392",
        "393",
        "394",
        "395",
        "396",
        "397",
        "398",
        "399",
        "400",
        "401",
        "402",
        "403",
        "404",
        "405",
        "406",
        "407",
        "408",
        "409",
        "410",
        "411",
        "412",
        "413",
        "414",
        "415",
        "416",
        "417",
        "418",
        "419",
        "420",
        "421",
        "422",
        "423",
        "424",
        "425",
        "426",
        "427",
        "428",
        "429",
        "430",
        "431",
        "432",
        "433",
        "434",
        "435",
        "436",
        "437",
        "438",
        "439",
        "440",
        "441",
        "442",
        "443",
        "444",
        "445",
        "446",
        "447",
        "448",
        "449",
        "450",
        "451",
        "452",
        "453",
        "454",
        "455",
        "456",
        "457",
        "458",
        "459",
        "460",
        "461",
        "462",
        "463",
        "464",
        "465",
        "466",
        "467",
        "468",
        "469",
        "470",
        "471",
        "472",
        "473",
        "474",
        "475",
        "476",
        "477",
        "478",
        "479",
        "480",
        "481",
        "482",
        "483",
        "484",
        "485",
        "486",
        "487",
        "488",
        "489",
        "490",
        "491",
        "492",
        "493",
        "494",
        "495",
        "496",
        "497",
        "498",
        "499"
    };

        // Lookup table last index.
        const int _top = 499;

        public static string ToStringLookup(this int value)
        {
            // See if the integer is in range of the lookup table.
            // ... If it is present, return the string literal element.
            if (value >= 0 &&
                value <= _top)
            {
                return _cache[value];
            }
            // Fall back to ToString method.
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }


}