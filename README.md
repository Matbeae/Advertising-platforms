# Advertising-platforms
Веб сервис, позволяющий хранить и возвращать списки рекламных площадок для заданной локации в запросе

## Требования

* Windows
* Visual Studio с поддержкой **.NET Framework (4.7.2 или выше)**
* Postman или curl для проверки запросов

## Установка и запуск

1. Склонируйте репозиторий или скачайте код:

   ```bash
   git clone https://github.com/username/ad-platforms-service.git
   ```
2. Откройте проект в Visual Studio.
3. Убедитесь, что у вас выбран **Console App (.NET Framework)**.
4. Соберите и запустите проект.
5. В консоли появится сообщение:

   ```
   Сервер запущен на http://localhost:5000/
   ```

---

## Формат входного файла

Файл с рекламными площадками должен быть в **UTF-8** и иметь структуру:

```
Площадка1:/ru
Площадка2:/ru/svrd/revda,/ru/svrd/pervik
Площадка3:/ru/msk,/ru/permobl,/ru/chelobl
Площадка4:/ru/svrd
```
(Файл Fi.txt в репозитории)

---

## Методы API

### 1. Загрузка данных

**POST /upload**

Загружает файл и полностью перезаписывает данные в памяти.

**Postman:**

* Method: `POST`
* URL: `http://localhost:5000/upload`
* Body → binary → выбрать файл `ads.txt`

**Ответ:**

```json
{"message":"Data uploaded successfully"}
```

---

### 2. Поиск рекламных площадок

**GET /search?location=/ваша/локация**

Возвращает список площадок, которые действуют в указанной локации или её предках.

**Пример запроса:**

```
GET http://localhost:5000/search?location=/ru/svrd/revda
```

**Ответ:**

```json
["Яндекс.Директ","Ревдинский рабочий","Крутая реклама"]
```
