using Enviro;
using UnityEngine;

public class RandomEnviroManager : MonoBehaviour {
    public static RandomEnviroManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    
    public void RandomizeTime()
    {
        if(EnviroManager.instance.Time == null)
            return;

        float randomHour = Random.Range(0f, 24f);  // 0시에서 24시 사이의 랜덤 시간 설정
        float randomMinutes = Random.Range(0f, 60f);  // 더 정밀한 시간 설정을 위한 랜덤 분
        EnviroManager.instance.Time.SetTimeOfDay(randomHour + randomMinutes / 60f);  // 시간을 설정



        Debug.Log($"설정된 시간: {randomHour}시 {(int)randomMinutes}분");
    }

    public void RandomizeWeather()
    {
        if(EnviroManager.instance.Weather != null)
        {
            // 날씨 랜덤화
            var weatherPresets = EnviroManager.instance.Weather.Settings.weatherTypes;  // 사용 가능한 날씨 프리셋 목록
            if (weatherPresets.Count > 0)
            {
                int randomWeatherIndex = Random.Range(0, weatherPresets.Count);  // 프리셋 중 하나를 랜덤 선택
                EnviroWeatherType randomWeather = weatherPresets[randomWeatherIndex];  
                EnviroManager.instance.Weather.ChangeWeather(randomWeather);  // 랜덤 날씨로 변경
            }            
            Debug.Log($"설정된 날씨: {EnviroManager.instance.Weather.targetWeatherType.name}");
        }
    }
}