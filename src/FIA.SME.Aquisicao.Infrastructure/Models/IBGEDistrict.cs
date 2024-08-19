namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class IBGEDistrict
    {
        public int id                       { get; set; }
        public string nome                  { get; set; }
        public IBGEDistrictCity municipio   { get; set; }

        public class IBGEDistrictCity
        {
            public int id                                   { get; set; }
            public string nome                              { get; set; }
            public IBGEDistrictCityMicroregion microrregiao { get; set; }

            public class IBGEDistrictCityMicroregion
            {
                public int id                                   { get; set; }
                public string nome                              { get; set; }
                public IBGEDistrictCityMesoregion mesorregiao   { get; set; }

                public class IBGEDistrictCityMesoregion
                {
                    public int id                   { get; set; }
                    public string nome              { get; set; }
                    public IBGEDistrictCityState UF { get; set; }
                }

                public class IBGEDistrictCityState
                {
                    public int id       { get; set; }
                    public string nome  { get; set; }
                    public string sigla { get; set; }
                }
            }
        }
    }
}
