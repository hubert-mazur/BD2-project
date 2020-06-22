use testDB;
GO

IF OBJECT_ID('create_points_table') IS NOT NULL
	DROP PROCEDURE create_points_table;
	
IF OBJECT_ID('create_polygon_table') IS NOT NULL
	DROP PROCEDURE create_polygon_table;
	
IF OBJECT_ID('clear_points_table') IS NOT NULL
	DROP PROCEDURE clear_points_table;
	
IF OBJECT_ID('clear_polygons_table') IS NOT NULL
	DROP PROCEDURE clear_polygons_table;
	
IF OBJECT_ID('fill_polygon') IS NOT NULL
	DROP PROCEDURE fill_polygon;
GO

CREATE PROC create_points_table 
as
BEGIN
CREATE TABLE points (id int IDENTITY(1,1) PRIMARY KEY, point dbo.Point)
END;

GO

CREATE PROC create_polygon_table
as
BEGIN
CREATE TABLE polygon (id INT IDENTITY(1,1) PRIMARY KEY, pol dbo.Polygon);
END;

GO
CREATE PROC clear_points_table
as
BEGIN
DROP TABLE points;
END;

GO
CREATE PROC clear_polygons_table
as
BEGIN
DROP TABLE polygon;
END;

GO

CREATE PROC fill_polygon
as
BEGIN

DECLARE @pnt dbo.Point;

DECLARE curs CURSOR
	LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR
SELECT point FROM points;

OPEN curs
FETCH NEXT FROM curs INTO @pnt
WHILE @@FETCH_STATUS = 0
BEGIN
UPDATE polygon set pol.Load = @pnt;
FETCH NEXT FROM curs INTO @pnt;
END
CLOSE curs;
DEALLOCATE curs;
END;

