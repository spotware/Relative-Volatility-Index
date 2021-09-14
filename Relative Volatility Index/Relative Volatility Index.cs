// -------------------------------------------------------------------------------------------------------------------------------------------
//
//    Relative Volatility Index (RVI) is a volatility indicator that was developed by Donald Dorsey to indicate the direction of volatility.
//
// -------------------------------------------------------------------------------------------------------------------------------------------

using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class RelativeVolatilityIndex : Indicator
    {
        private StandardDeviation _std;

        private MovingAverage _upperMa, _lowerMa;

        private IndicatorDataSeries _upperMaSource, _lowerMaSource;

        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("Std Periods", DefaultValue = 10, MinValue = 1)]
        public int StdPeriods { get; set; }

        [Parameter("MA Periods", DefaultValue = 14, MinValue = 1)]
        public int MaPeriods { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType MaType { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _std = Indicators.StandardDeviation(Source, StdPeriods, MaType);

            _upperMaSource = CreateDataSeries();
            _lowerMaSource = CreateDataSeries();

            _upperMa = Indicators.MovingAverage(_upperMaSource, MaPeriods, MaType);
            _lowerMa = Indicators.MovingAverage(_lowerMaSource, MaPeriods, MaType);
        }

        public override void Calculate(int index)
        {
            var diff = Source[index] - Source[index - 1];

            _upperMaSource[index] = diff <= 0 ? 0 : _std.Result[index];
            _lowerMaSource[index] = diff > 0 ? 0 : _std.Result[index];

            Result[index] = _upperMa.Result[index] / (_upperMa.Result[index] + _lowerMa.Result[index]) * 100;
        }
    }
}