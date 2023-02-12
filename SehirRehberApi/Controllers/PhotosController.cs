using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SehirRehberApi.Data;
using SehirRehberApi.Dto;
using SehirRehberApi.Helpers;
using SehirRehberApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SehirRehberApi.Controllers
{
    [Route("api/cities/{cityId}/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private IAppRepository _appRepository;
        private IMapper _mapper;
        IOptions<CloudinarySettings> _cloudinaryConfig;

        private Cloudinary _cloudinary;
       

        public PhotosController(IAppRepository appRepository, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _appRepository = appRepository;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account account = new Account(_cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);


        }

        [HttpPost]
        
        public ActionResult AddPhotoForCity(int cityId,[FromForm]PhotoForCreationDto photoForCreationDto)
        {
            var city = _appRepository.GetCityById(cityId); // sehir cekirik

            if (city==null)
            {
                return BadRequest("Could not fouund city");//sehir yoksa bad request
            }
            //Console.WriteLine(User);
          //  var currentUser =int.Parse( User.FindFirst(ClaimTypes.NameIdentifier).Value);

          //  if (currentUser!=city.UserId)
          //  {
          //     return Unauthorized();//istfadeci userid leri farkli ise islem yapamiz
           // }
            var file = photoForCreationDto.File;//file goturuk


            var uploadResult = new ImageUploadResult();

            if (file.Length>0)
            {
                using (var stream= file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File= new FileDescription(file.Name,stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);//upload islemi
                }

            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();//uplad edilenden sora urlin gotur

            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);//photoya map et

            photo.City = city;//sehirn adini yaz
            if (!city.Photos.Any(p=>p.IsMain))
            {
                photo.IsMain = true;//is manin tru olub olmamaqin yoxla
            }

            city.Photos.Add(photo);//citinin photolarina ekle

            if (_appRepository.SaveAll())

            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }

            return BadRequest("Colud not add the photo");


        }
        [HttpGet("{id}",Name ="GetPhoto")]
        public ActionResult GetPhoto(int id)
        {
            var photoFromDb = _appRepository.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromDb);

            return Ok(photo);


        }
      
    }
}
