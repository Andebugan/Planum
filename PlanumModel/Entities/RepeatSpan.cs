namespace Planum.Model.Entities
{
    public struct RepeatSpan: IComparable
    {
        int years = 0;
        int months = 0;

        public TimeSpan Span { get; set; } = TimeSpan.Zero;

        public int Years
        {
            get => years;
            set => years = value;
        }

        public int Months
        {
            get => months;
            set
            {
                years += (int)(value / 12);
                months = value % 12; 
            }
        }

        public RepeatSpan() { }
        public RepeatSpan(int years, int months, TimeSpan span)
        {
            Years = years;
            Months = months;
            Span = span;
        }
        public override string ToString() => $"{Years} {Months} {Span.ToString(@"d\.h\:m")}";

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            return Equals((RepeatSpan)obj);
        }

        public bool Equals(PlanumTask compared) => this.GetHashCode() == GetHashCode();
        public override int GetHashCode() => HashCode.Combine(Years.GetHashCode(), Months.GetHashCode(), Span.GetHashCode());

        public int CompareTo(object? obj)
        {
            if (obj is null)
                return -1;
            var compared = (RepeatSpan)obj;

            var summary = Years * 365 + Months * 30 + Span.Days + Span.Hours * 0.1 + Span.Minutes * 0.01;
            var comparedSummary = compared.Years * 365 + compared.Months * 30 + compared.Span.Days + compared.Span.Hours * 0.1 + compared.Span.Minutes * 0.01;
            return summary.CompareTo(comparedSummary);
        }
    }
}
