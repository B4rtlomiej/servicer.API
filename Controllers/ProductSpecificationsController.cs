﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using servicer.API.Data;
using servicer.API.Dtos;
using servicer.API.Helpers;
using servicer.API.Models;

namespace servicer.API.Controllers
{
    [ServiceFilter(typeof(SetLastActive))]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSpecificationsController : ControllerBase
    {
        private readonly IServicerRepository _repository;
        private readonly IMapper _mapper;
        public ProductSpecificationsController(IServicerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductSpecifications([FromQuery]ProductSpecificationParams productSpecificationParams)
        {
            var productSpecifications = await _repository.GetProductSpecifications(productSpecificationParams);
            
            var productSpecificationsToReturn = _mapper.Map<IEnumerable<ProductSpecificationForListDto>>(productSpecifications);

            Response.AddPagination(productSpecifications.CurrentPage, productSpecifications.PageSize, productSpecifications.TotalCount, productSpecifications.TotalPages);
            return Ok(productSpecificationsToReturn);
        }

        [HttpGet("{id}", Name = "GetProductSpecification")]
        public async Task<IActionResult> GetProductSpecification(int id)
        {
            var productSpecification = await _repository.GetProductSpecification(id);
            var productSpecificationToReturn = _mapper.Map<ProductSpecificationForDetailDto>(productSpecification);

            return Ok(productSpecificationToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductSpecification(ProductSpecificationForSendDto productSpecificationForSendDto)
        {
            var productSpecificationToCreate = _mapper.Map<ProductSpecification>(productSpecificationForSendDto);

            var createdProductSpecification = await _repository.CreateProductSpecification(productSpecificationToCreate);
            var productSpecificationsToReturn = _mapper.Map<ProductSpecificationForDetailDto>(createdProductSpecification);

            return CreatedAtRoute("GetProductSpecification", new { controller = "ProductSpecifications", id = createdProductSpecification.Id }, productSpecificationsToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductSpecification(int id, ProductSpecificationForUpdateDto productSpecificationForUpdateDto)
        {
            var productSpecificationFromRepo = await _repository.GetProductSpecification(id);

            try {
                _mapper.Map(productSpecificationForUpdateDto, productSpecificationFromRepo);
                await _repository.SaveAll();
                return NoContent();
            } catch (Exception) {
                return NoContent();
            }
        }

        [HttpPut("{id}/changeisactive")]
        public async Task<IActionResult> ChangeIsActive(int id)
        {
            var productSpecificationToChangeIsActive = await _repository.GetProductSpecification(id);
            productSpecificationToChangeIsActive.IsActive = !productSpecificationToChangeIsActive.IsActive;

            await _repository.SaveAll();

            return Ok(new
            {
                message = "Zmieniono flagę aktywności."
            });
        }
    }
}