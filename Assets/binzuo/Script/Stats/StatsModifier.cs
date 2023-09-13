namespace binzuo
{
        public enum StatsModifierType
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMulti = 300

    }
    public class StatsModifier
    {
        public readonly float value;
        public readonly StatsModifierType type;
        public readonly int order;
        public readonly object source;

        public StatsModifier(float value, StatsModifierType type, int order, object source)
        {
            this.value = value;
            this.type = type;
            this.order = order;
            this.source = source;
        }

        public StatsModifier(float value, StatsModifierType type) : this(value, type, (int)type, null)
        {
        }

        public StatsModifier(float value, StatsModifierType type, int order) : this(value, type, order, null)
        {
        }

        public StatsModifier(float value, StatsModifierType type, object source) : this(value, type, (int)type, source)
        {
        }

    }
}