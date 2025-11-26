namespace GenericalAPI.Tests.Unit.TestUtils.BaseTests;

/// <summary>
/// Base simple para builders de datos de prueba, permitiendo un Build fluido y reutilizable.
/// </summary>
public abstract class TestDataBuilder<T>
{
    public abstract T Build();

    protected TBuilder With<TBuilder>(Action<TBuilder> apply)
        where TBuilder : TestDataBuilder<T>
    {
        apply((TBuilder)this);
        return (TBuilder)this;
    }
}
