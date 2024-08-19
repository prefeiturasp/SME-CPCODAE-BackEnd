using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class LocationRegion
    {
        #region [ Construtores ]

        internal LocationRegion(LocalidadeRegiao? localidadeRegiao)
        {
            if (localidadeRegiao == null)
                return;

            this.id = localidadeRegiao.id;
            this.city_name = localidadeRegiao.municipio;
            this.state_acronym = localidadeRegiao.uf;
            this.imediate_region_id = localidadeRegiao.regiao_imediata_id;
            this.intermediate_region_id = localidadeRegiao.regiao_intermediaria_id;
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public int id                       { get; private set; }
        public string city_name             { get; private set; } = String.Empty;
        public string state_acronym         { get; private set; } = String.Empty;
        public int imediate_region_id       { get; private set; } = 0;
        public int intermediate_region_id   { get; private set; } = 0;

        #endregion [ FIM - Propriedades ]
    }
}
