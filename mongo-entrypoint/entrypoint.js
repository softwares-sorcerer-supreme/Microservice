var db = connect("mongodb://admin:123456@localhost:27017/admin");
// db = db.getSiblingDB('cartdb'); // we can not use "use" statement here to switch db
db = new Mongo().getDB('cartdb'); // or we can use this way to switch db
db.createUser(
    {
        user: "cartdb_user",
        pwd: "123456",
        roles: [ { role: "readWrite", db: "cartdb"} ],
    }
)