# Domain Events
The following is a list of domain events that are published from the **Capability Service**. Events are published on the Kafka topic `build.capabilities`. Currently the following events are published:

## Event: capability_created
**Occures:** when a new capability has been created

**Example**:
```json
{
	"messageId": "ab509002-b295-46c5-80c0-3f0178174927",
	"type": "capability_created",
	"data": {
		"capabilityId": "0d03e3ad-2118-46b7-970e-0ca87b59a202",
		"capabilityName": "build.capabilities"
	}
}
```

## Event: member_joined_capability
**Occures:** when a member joins a capability

**Example**:
```json
{
	"messageId": "f791bc18-6eec-4e1c-ba48-c384b174f961",
	"type": "member_joined_capability",
	"data": {
		"capabilityId": "0d03e3ad-2118-46b7-970e-0ca87b59a202",
		"memberEmail": "johndoe@jdog.com"
	}
}
```

## Event: member_left_capability
**Occures:** when a member has left a capability

**Example**:
```json
{
	"messageId": "2f9ec6dc-8800-4ac0-ae21-a8c862c2d22f",
	"type": "member_left_capability",
	"data": {
		"capabilityId": "0d03e3ad-2118-46b7-970e-0ca87b59a202",
		"memberEmail": "johndoe@jdog.com"
	}
}
```
