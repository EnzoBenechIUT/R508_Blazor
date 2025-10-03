using AutoMapper;

namespace App.Mapper
{
    public class AutoMapperAdapter<TEntity, TDto> : IMapper<TEntity, TDto>
    {
        private readonly IMapper _mapper;

        public AutoMapperAdapter(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TDto ToDTO(TEntity entity)
        {
            return _mapper.Map<TDto>(entity);
        }

        public TEntity ToEntity(TDto dto)
        {
            return _mapper.Map<TEntity>(dto);
        }
    }
}
