namespace GastroApp.Models
{
    public class Coordinates
    {
        public Coordinates()
        {

        }
        public Coordinates(string latitude, string longitude, string address, string dataOne)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.address = address;
            this.DataOne = dataOne;
        }

        public string latitude { get; set; }
        public string longitude { get; set; }
        public string address { get; set; }
        public string DataOne { get; set; }
        public int NrZamowienia { get; set; }

    }
}
