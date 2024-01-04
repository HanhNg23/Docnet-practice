using DocnetCorePractice.Data;
using DocnetCorePractice.Data.Entity;
using DocnetCorePractice.Model;
using Serilog;
using ILogger = Serilog.ILogger;

namespace DocnetCorePractice.Services
{
    public interface ICaffeService
    {
        List<CaffeEntity> GetAllCaffeEntities();
        List<CaffeModel> GetAllCaffeModels();
        CaffeEntity AddNewCaffe(CaffeEntity entity);
        CaffeEntity UpdateExistedCaffe(CaffeEntity caffe);
        Boolean DeleteCaffe(CaffeEntity caffe);
    }
    public class CaffeService : ICaffeService
    {
        private readonly IInitData _initData; //inject the already IInitData instance 
        private List<CaffeEntity> _caffeEntities;
        private readonly ILogger _logger; 
        public CaffeService(InitData initData)
        {
            _initData = initData; 
            _caffeEntities = _initData.GetCurrentCaffeEntities();
            _logger = Log.Logger;
        }
        
        public List<CaffeEntity>? GetAllCaffeEntities()
        {
            List<CaffeEntity> caffeList = _initData.GetCurrentCaffeEntities();
            return caffeList;
        }

        public List<CaffeModel>? GetAllCaffeModels()
        {

            List<CaffeModel> caffeModels = new List<CaffeModel>();
            _caffeEntities.ForEach(e => {
                caffeModels.Add(new CaffeModel() {
                    Id = e.Id,
                    Name = e.Name,
                    Discount = e.Discount,
                    Price = e.Price,
                    Type = e.Type
                });
            });
            return caffeModels;
        }

        public CaffeEntity AddNewCaffe(CaffeEntity caffe)
        {

            _caffeEntities.Add(caffe);
            var addedCaffeEntity = _caffeEntities.FirstOrDefault(x => x.Id.Equals(caffe.Id));
            _logger.Information("New Caffe: {}", _caffeEntities);
            return addedCaffeEntity;

        }

        public CaffeEntity UpdateExistedCaffe(CaffeEntity caffe)
        {
            var oldCaffe = _caffeEntities.FirstOrDefault(x => x.Id.Equals(caffe.Id));
            oldCaffe = caffe;
            return AddNewCaffe(caffe);
        }

        public Boolean DeleteCaffe(CaffeEntity caffe)
        {
            _caffeEntities.Remove(caffe);
            var deletedCaffe = _caffeEntities.FirstOrDefault(x => x.Id.Equals(caffe.Id));
            if (deletedCaffe == null)
                return true;
            else 
                return false;
        }

        
    }
}
