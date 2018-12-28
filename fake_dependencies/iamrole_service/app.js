const express = require("express");
const app = express();
const port = 50800;

app.use(express.json());

app.post("/api/roles", (req, res) => {

console.log("Body: " + JSON.stringify(req.body));

    const roleName = req.body.name;
    const result = {
        roleArn: `arn:fakeiam:${roleName}`
    };

    console.log(`POST> received role name: "${roleName}" --- sending respose: ${JSON.stringify(result)}`);

    res.send(result);
});

app.listen(port, () => {
    console.log(`Fake IAM Role Service is running on port ${port}`);
})