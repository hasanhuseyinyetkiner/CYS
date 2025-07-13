using System;
using System.Threading.Tasks;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using System.Text;

namespace CYS.ComponentManager
{
    public class BluetoothScaleManager
    {
        private BluetoothClient _client;
        private BluetoothDeviceInfo _connectedDevice;
        private bool _isConnected;

        public BluetoothScaleManager()
        {
            _client = new BluetoothClient();
            _isConnected = false;
        }

        public async Task<BluetoothDeviceInfo[]> ScanDevices()
        {
            try
            {
                return _client.DiscoverDevices();
            }
            catch (Exception ex)
            {
                throw new Exception("Bluetooth cihazları taranırken hata oluştu: " + ex.Message);
            }
        }

        public async Task<bool> ConnectToDevice(BluetoothDeviceInfo device)
        {
            try
            {
                if (_isConnected)
                {
                    _client.Close();
                    _isConnected = false;
                }

                _client.Connect(device.DeviceAddress, BluetoothService.SerialPort);
                _connectedDevice = device;
                _isConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Bluetooth cihazına bağlanırken hata oluştu: " + ex.Message);
            }
        }

        public async Task<double> ReadWeight()
        {
            if (!_isConnected || _client == null)
            {
                throw new Exception("Terazi bağlı değil!");
            }

            try
            {
                var stream = _client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                // Veriyi parse et - bu kısım terazinin gönderdiği veri formatına göre düzenlenmelidir
                if (double.TryParse(data, out double weight))
                {
                    return weight;
                }
                throw new Exception("Ağırlık verisi okunamadı!");
            }
            catch (Exception ex)
            {
                throw new Exception("Ağırlık okuma hatası: " + ex.Message);
            }
        }

        public void Disconnect()
        {
            if (_isConnected)
            {
                _client.Close();
                _isConnected = false;
            }
        }
    }
} 