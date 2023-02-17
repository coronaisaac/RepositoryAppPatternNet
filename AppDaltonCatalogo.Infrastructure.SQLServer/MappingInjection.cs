using AppDaltonCatalogo.Infrastructure.SQL.Mapping;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDaltonCatalogo.Infrastructure.SQL
{
    public class MappingInjection
    {
        private static MapperConfiguration mapperConfiguration;
        private static readonly MappingInjection? Instance = null;
        private MappingInjection() =>
            mapperConfiguration = new MapperConfiguration(config => config.AddProfile(new MappingProfileIn()));
        public static MappingInjection Injecction =>
            Instance ?? new MappingInjection();
        public IMapper AccessMapProfile =>
            mapperConfiguration.CreateMapper();
    }
}

