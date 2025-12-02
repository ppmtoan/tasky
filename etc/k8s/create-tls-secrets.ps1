mkcert "moduletest.dev" "*.moduletest.dev" 
kubectl create namespace moduletest
kubectl create secret tls -n moduletest moduletest-tls --cert=./moduletest.dev+1.pem  --key=./moduletest.dev+1-key.pem