﻿using System.ComponentModel.DataAnnotations.Schema;
using WebStore.Domain.Entities.Base;
using WebStore.Domain.Entities.Base.Interfaces;

namespace WebStore.Domain.Entities
{
	/// <summary>
	/// Товар
	/// </summary>
	[Table("Products")]
	public class Product : NamedEntity, IOrderedEntity
	{
		public int Order { get; set; }

		public int SectionId { get; set; }

		[ForeignKey(nameof(SectionId))] // Указываем связь по внешнему ключу для навигационного свойства
		public virtual Section Section { get; set; }

		public int? BrandId { get; set; }

		[ForeignKey(nameof(BrandId))]
		public virtual Brand Brand { get; set; }

		public string ImageUrl { get; set; }

		[Column(TypeName = "decimal(18, 2)")]
		public decimal Price { get; set; }
	}
}
