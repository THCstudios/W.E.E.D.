drop table if exists o_objects;
create table o_objects(o_id decimal primary key,o_name string);
insert into o_objects values ('0','Game');
create table if not exists e_entries(e_name string primary key,e_value string,e_o_id decimal,foreign key(e_o_id) references o_objects(o_id) on update cascade on delete cascade);
insert into e_entries values ('SharedMemoryInfo','2','0');
insert into e_entries values ('GameInfo','1','0');
insert into o_objects values ('1','GameInfo');
create table if not exists e_entries(e_name string primary key,e_value string,e_o_id decimal,foreign key(e_o_id) references o_objects(o_id) on update cascade on delete cascade);
insert into e_entries values ('Name','Test','1');
insert into e_entries values ('Level','3','1');
insert into e_entries values ('Host','THC','1');
insert into o_objects values ('2','SharedMemoryInfo');
create table if not exists e_entries(e_name string primary key,e_value string,e_o_id decimal,foreign key(e_o_id) references o_objects(o_id) on update cascade on delete cascade);
insert into e_entries values ('Version','0.1beta','2');
insert into e_entries values ('Vendor','InfiniteReality','2');
insert into e_entries values ('ConnectionFormatter','JSON','2');
insert into o_objects values ('3','Level');
create table if not exists e_entries(e_name string primary key,e_value string,e_o_id decimal,foreign key(e_o_id) references o_objects(o_id) on update cascade on delete cascade);
insert into e_entries values ('Map','4','3');
insert into e_entries values ('Entities','6','3');
insert into o_objects values ('4','Map');
create table if not exists e_entries(e_name string primary key,e_value string,e_o_id decimal,foreign key(e_o_id) references o_objects(o_id) on update cascade on delete cascade);
insert into e_entries values ('Tiles','5','4');
insert into e_entries values ('Name','TestMap','4');
insert into e_entries values ('File','test_map.map','4');
insert into o_objects values ('5','Tiles');
create table if not exists e_entries(e_name string primary key,e_value string,e_o_id decimal,foreign key(e_o_id) references o_objects(o_id) on update cascade on delete cascade);
insert into e_entries values ('TileCount','2','5');
insert into e_entries values ('Tile_1','101','5');
insert into e_entries values ('Tile_0','100','5');
insert into o_objects values ('6','Entities');
create table if not exists e_entries(e_name string primary key,e_value string,e_o_id decimal,foreign key(e_o_id) references o_objects(o_id) on update cascade on delete cascade);
insert into e_entries values ('EntityCount','2','6');
insert into e_entries values ('Entity_1','1001','6');
insert into e_entries values ('Entity_0','1000','6');
insert into o_objects values ('100','Tile_0');
create table if not exists e_entries(e_name string primary key,e_value string,e_o_id decimal,foreign key(e_o_id) references o_objects(o_id) on update cascade on delete cascade);
insert into e_entries values ('Y','0','100');
insert into e_entries values ('X','0','100');
insert into e_entries values ('Terrain','0','100');
insert into e_entries values ('Levitation','0','100');
insert into o_objects values ('101','Tile_1');
create table if not exists e_entries(e_name string primary key,e_value string,e_o_id decimal,foreign key(e_o_id) references o_objects(o_id) on update cascade on delete cascade);
insert into e_entries values ('Y','0','101');
insert into e_entries values ('X','1','101');
insert into e_entries values ('Terrain','0','101');
insert into e_entries values ('Levitation','0','101');
insert into o_objects values ('1000','Entity_0');
create table if not exists e_entries(e_name string primary key,e_value string,e_o_id decimal,foreign key(e_o_id) references o_objects(o_id) on update cascade on delete cascade);
insert into e_entries values ('Tile','100','1000');
insert into e_entries values ('Name','Townhall','1000');
insert into e_entries values ('HP','1500','1000');
insert into e_entries values ('EntityType','100','1000');
insert into e_entries values ('AP','20','1000');
insert into o_objects values ('1001','Entity_1');
create table if not exists e_entries(e_name string primary key,e_value string,e_o_id decimal,foreign key(e_o_id) references o_objects(o_id) on update cascade on delete cascade);
insert into e_entries values ('Tile','101','1001');
insert into e_entries values ('Name','Crossbow','1001');
insert into e_entries values ('HP','150','1001');
insert into e_entries values ('EntityType','1000','1001');
insert into e_entries values ('AP','10','1001');
