db = db.getSiblingDB('mmorpg');

db.createUser({
    user: "adminuser",
    pwd: "adminpassword",
    roles: [
        { role: "readWrite", db: "mmorpg" },
        { role: "dbAdmin", db: "mmorpg" },
        { role: "userAdmin", db: "mmorpg" }
    ]
});

print("✅ Created user adminuser in mmorpg database");
