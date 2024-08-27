using System;
using System.Collections.Generic;

namespace Reporting.Application.Command.Model
{
    public class AssetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizeName { get; set; }
        public IEnumerable<AttributeDto> Attributes { get; set; } = new List<AttributeDto>();
    }
}