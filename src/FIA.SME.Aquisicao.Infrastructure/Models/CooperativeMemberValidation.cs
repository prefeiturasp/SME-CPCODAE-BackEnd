namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class CooperativeMemberValidation
    {
        public CooperativeMemberValidation(Guid id, string cpf, string dap_caf_code, string name, decimal total_year_supplied_value)
        {
            this.id = id;
            this.cpf = cpf;
            this.dap_caf_code = dap_caf_code;
            this.name = name;
            this.lineNumber = 0;
            this.total_year_supplied_value = total_year_supplied_value;
            this.message = "OK";
            this.is_success = true;
        }

        public CooperativeMemberValidation(string dap_caf, int lineNumber)
        {
            this.id = null;
            this.cpf = null;
            this.dap_caf_code = dap_caf;
            this.name = null;
            this.lineNumber = lineNumber;
            this.total_year_supplied_value = 0;
            this.message = $"Cooperado com a DAP/CAF {dap_caf} não está cadastrado";
            this.is_success = false;
        }

        public Guid? id                             { get; private set; }
        public string? cpf                          { get; private set; } = String.Empty;
        public string? dap_caf_code                 { get; private set; } = String.Empty;
        public string? name                         { get; private set; }
        public int lineNumber                       { get; private set; }
        public decimal total_year_supplied_value    { get; private set; }

        public string message                       { get; private set; } = String.Empty;
        public bool is_success                      { get; private set; } = true;
    }
}
