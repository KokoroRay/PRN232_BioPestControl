curl -sS -X POST http://identity-service.biopest.svc.cluster.local/api/auth/login -H 'Content-Type: application/json' -d '{"email": "admin@biopest.com", "password": "Admin@123"}' > /tmp/login.json
TOKEN=$(cat /tmp/login.json | grep -o '"token":"[^"]*' | cut -d'"' -f4)
curl -sS -X GET http://ordering-service.biopest.svc.cluster.local/api/cart -H "Authorization: Bearer $TOKEN"
