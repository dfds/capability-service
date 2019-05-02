const express = require("express");
const app = express();
const port = 50801;

app.use(express.json());

app.post("/api/roles", (req, res) => {
    const roleArn = req.body.roleArn;
    const roleName = req.body.roleName;
    
    console.log(`POST> received arn: "${roleArn}" and name: "${roleName}" --- sending 200 OK`);

    res.sendStatus(200);
});

app.post("/api/events", (req, res) => {
    const roleArn = req.body.roleArn;
    const capabilityName = req.body.capabilityName;
    
    console.log(`POST> received arn: "${roleArn}" and name: "${capabilityName}" --- sending 200 OK`);

    res.sendStatus(200);
});

app.listen(port, () => {
    console.log(`Fake RoleMapper Service is running on port ${port}`);
});