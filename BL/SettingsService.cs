using DAL;

namespace BL
{
    public class SettingsService
    {
        private readonly SettingsRepository _settingsRepository;

        public SettingsService()
        {
            _settingsRepository = new SettingsRepository();
        }

        public decimal GetRatePerHour()
        {
            return _settingsRepository.GetRatePerHour();
        }

        public void UpdateRatePerHour(decimal newRate)
        {
            if (newRate <= 0)
            {
                throw new Exception("Rate per hour must be greater than zero.");
            }

            _settingsRepository.UpdateRatePerHour(newRate);
        }
    }
}
