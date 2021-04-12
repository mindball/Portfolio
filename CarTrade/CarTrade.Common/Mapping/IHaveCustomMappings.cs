using AutoMapper;

namespace CarTrade.Common.Mapping
{
    public interface IHaveCustomMappings 
    {
        void ConfigureMapping(Profile mapper);
    }
}
