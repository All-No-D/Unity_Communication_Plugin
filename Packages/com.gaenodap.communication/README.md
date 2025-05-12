# 설명

개노답 팀의 Http 통신과 웹소켓 통신에 관한 유틸리티 패키지입니다.

<br>

# Http 통신 사용법

1. api 정보 참조를 위해 Assets/Resources/Config/api_config.json 추가<br>
```json
{
  "baseUri": "http://api.example.com"
}
```

2. uri가 http 프로토콜일 경우 유니티 프로젝트에서 Edit > Project Settings > Player > Other Settings > Configuration > Allow downloads over HTTP* 옵션을 "Always allowed"로 변경

3. Api 통신 성공 시 반환하는 Json Object 값을 받기 위한 직렬화된 클래스 생성
```csharp
using System;

[Serializable]
class Position
{
    public int x;
    public int y;
}

[Serializable]
class Item
{
    public string ID;
    public string Name;
    public string Quantity;
    public int CurrentDurabilityPoints;
    public Position Position;
}
```

4. MonoBehaviour를 상속받은 클래스를 생성하고 3에서 만든 클래스와 연계하여 해당 클래스 내에서 Request 함수 사용
함수 설명
```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

public static void Request<T>(
    MonoBehaviour behaviour, // 코루틴 기반 api 호출을 위한 MonoBehaviour 클래스
    string path, // 1에서 설정한 uri와 연계하여 사용할 api path
    Method method, // api 호출에 필요한 Method(GET, POST, PUT, DELETE)
    Dictionary<string, string> header = null, // api 호출에 필요한 Header
    Dictionary<string, object> body = null, // api 호출에 필요한 Body, GET에 경우 path에 Body 내용이 포함될 경우 null
    int timeoutSeconds = 10, // api 호출 시 timeout 판정을 위한 시간, 단위: 초
    int maxRetryCount = 0, // api 호출 시 timeout에 의한 재시도 횟수
    Action<HttpResult<T>> onSuccess = null, // api 호출 성공 시의 Callback 함수
    Action<string> onFailure = null // api 호출 실패 시의 Callback 함수
)
```

사용 예시)
```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

public class HttpTest : MonoBehaviour
{
    private void Start()
    {
        string userId = "test";

        Gaenodap.Http.Client.Request<Item[]>(
            this,
            $"/keepout/api/test/{userId}/bag",
            Gaenodap.Http.Method.GET,
            header: new Dictionary<string, string>()
                {
                    { "Authorization", "test" }
                },
            body: new Dictionary<string, object>(),
            onSuccess: result =>
                {
                    Debug.Log("status code = " + result.statusCode);
                    Debug.Log("message = " + result.msg);
                    int i = 1;
                    foreach (Item item in result.data)
                    {
                        Debug.Log(i + "th");
                        Debug.Log("\t ID = " + item.ID);
                        Debug.Log("\t Name = " + item.Name);
                        Debug.Log("\t Quantity = " + item.Quantity);
                        Debug.Log("\t CurrentDurabilityPoints = " + item.CurrentDurabilityPoints);
                        Debug.Log("\t x = " + item.Position.x);
                        Debug.Log("\t y = " + item.Position.y);
                        i++;
                    }
                },
            onFailure: message => 
                {
                    Debug.Log("Fail message: " + result);
                });
    }
}
```