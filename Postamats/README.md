[Вернутся в оглавление](../Documentation/responsive-content.md)
# Назначение и место в общей структуре
Проект на Python (исполняемый файл скрипта) предназначен для запуска из процедуры БД по расписанию. Конфигурация описана ниже

## Примечания к обработчику Доставщиков
Проект на данный момент выполняет функцию получения данных с API ~~разных~~ доставщикa их стандартизирование после чего загрузка в БД.
Проект написан полностью на питоне и использует:
- Библиотеку pyodbc (```pip install pyodbc```)
- Библиотеку requests (```pip install requests```)
- Базу данных(см. подключение ниже)
- На данный момент только API 5Post

## Конфигурация и добавление небольших изменений
Вся конфигурация настраивается для одного доставщика в нашем случаи все конфиги находится в _5PostApi.py

С 9 по 14 строку настраиваются API ключи для 5Post, тестовые и продуктовые
16 строка ```VCIDWhereQuery``` - пока ни для чего не используется(Будет использоватся для новых доставщиков)
17 строка подключение к БД, пример 
```py
Database("192.168.0.11","dbName","sqlUsername","sqlPassword","[Postamats]", "[Postamats-PointsVC]")
```
Последних два параметра это наименования таблиц
Таблица Postamats используется для записи всех доступных постаматов и постоянно очищается
 -- Ниже представлен скрипт для минимально-рабочей таблицы
```sql
CREATE TABLE [dbo].[Postamats](
	[ID_NUM] [int] IDENTITY(1,1) NOT NULL,
	[id] [nvarchar](10) NOT NULL,
	[pvz_id] [nvarchar](10) NULL,
	[latitude] [float] NULL,
	[longtitude] [float] NULL,
	[company] [nvarchar](3) NULL,
	[name] [nvarchar](256) NULL,
	[station] [nvarchar](256) NULL,
	[metro] [nvarchar](256) NULL,
	[address] [nvarchar](256) NULL,
	[worktime] [nvarchar](256) NULL,
	[phone] [nvarchar](256) NULL,
	[payment] [nvarchar](256) NULL,
	[limit] [nvarchar](256) NULL,
	[postamat] [bit] NULL,
	[returns] [bit] NULL,
	[record_date] [datetime] NULL
) ON [PRIMARY]
```
Таблица Postamats-PointsVC используется для сохранения и связыванния кастомных ID внутри нашей системы пример ID ```5B001```
Так же таблица **НЕ** очищается и поэтому имеется на всякий случай с полем последнего использования
```sql
CREATE TABLE [dbo].[Postamats-PointsVC](
	[VC_ID] [varchar](8) NULL,
	[UUID] [varchar](50) NULL,
	[LastTimeUse] [datetime] NULL
) ON [PRIMARY]
GO
```

## Возможные проблемы
Может возникнуть проблемы что метод ```GetAllPoints()``` или ```GetPoint()``` не поддерживают ```NULL``` в БД для исправления этйо проблемы можно использовать пустые строки, что бы их включить можно зайти в ```Database.py``` 66-67 строка и изменить поменять комментирование 
