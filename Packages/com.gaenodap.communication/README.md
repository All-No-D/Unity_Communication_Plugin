# 설명

개노답 팀의 Http 통신과 웹소켓 통신에 관한 유틸리티 패키지입니다.

<br>

# 프로젝트에 패키지 추가 방법

프로젝트 루트 폴더/Packages/manifest.json에서 dependencies에 다음 패키지 추가("#v1.0.0" 은 버전명으로 패키지 업데이트시 버전 변경 필요)

"com.gaenodap.communication": "https://github.com/All-No-D/Unity_Communication_Plugin.git?path=Packages/com.gaenodap.communication#v1.0.0"

# Http 통신 사용법

1. api 정보 참조를 위해 Assets/Resources/Config/api_config.json 추가<br>
```json
{
  "baseUri": "http://api.example.com"
}
```

2. uri가 http 프로토콜일 경우 유니티 프로젝트에서 Edit > Project Settings > Player > Other Settings > Configuration > Allow downloads over HTTP* 옵션을 "Always allowed"로 변경

3. API 통신을 요청하거나 요청 성공 시에 사용할 직렬화된 클래스 생성
* API 요청에 사용되는 클래스는 HttpBody를 반드시 상속하고 변수명은 API 문서의 형식과 일치시켜야 함(대소문자 구분됨)
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

class DeleteItemInBagBody : Gaenodap.HttpBody
{
    public int id;
    public int decrementCount;

    public DeleteItemInBagBody(int id, int decrementCount)
    {
        this.id = id;
        this.decrementCount = decrementCount;
    }
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
    Dictionary<string, string> header = null, // api 호출에 필요한 Header<br>
    // api 호출에 필요한 Body, GET에 경우 path에 Body 내용이 포함될 경우 null
    // HttpBody를 상속받은 데이터 클래스 정의한 후 해당 클래스의 객체를 사용 => 내부 Item이 객체인 경우 [Serialize]를 사용하여 직렬화해야함
    HttpBody body = null,
    int timeoutSeconds = 10, // api 호출 시 timeout 판정을 위한 시간, 단위: 초
    int maxRetryCount = 0, // api 호출 시 timeout에 의한 재시도 횟수
    Action<HttpResult<T>> onSuccess = null, // api 호출 성공 시의 Callback 함수
    Action<string> onFailure = null // api 호출 실패 시의 Callback 함수
)
```

사용 예시 1)
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
            body: ,
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

사용 예시 2)
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
            "/keepout/api/test/inventory/item",
            Gaenodap.Http.Method.DELETE,
            header: new Dictionary<string, string>()
                {
                    { "Authorization", "test" }
                },
            body: new DeleteItemInBagBody(id: 1, decrementCount: 1),
            onSuccess: result =>
                {
                    Debug.Log("status code = " + result.statusCode);
                    Debug.Log("message = " + result.msg);

                    Debug.Log("item id = " + result.data.id);
                    Debug.Log("decrementCount = " + result.data.decrementCount);
                },
            onFailure: message => 
                {
                    Debug.Log("Fail message: " + result);
                });
    }
}
```