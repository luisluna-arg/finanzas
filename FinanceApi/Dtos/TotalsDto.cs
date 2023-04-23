namespace FinanceApi.Dtos;

public class TotalsDto {
    public TotalsDto() {

    }

    public DateTime TimeStamp { get; internal set; }
    public decimal? Total { get; internal set; }
}