# PlayOfferService

## Deploying Kubernetes Manifests Locally with Minikube

### Prerequisites
- [Install Minikube](https://minikube.sigs.k8s.io/docs/start/)
- [Install kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl/)

### Step-by-Step Guide

#### 1. Start Minikube
Start Minikube with sufficient resources.
```bash
minikube start --memory=4096 --cpus=2
```

#### 2. Apply kubernetes Manifests
Use kubectl to apply each of these YAML files. This will create the necessary Kubernetes resources.
```bash
kubectl apply -f redis.yml
kubectl apply -f pos_postgres_write.yml
kubectl apply -f pos_postgres_read.yml
kubectl apply -f pos_debezium.yml
kubectl apply -f pos_service.yml
```

#### 3. Check the status of pods/services
Check the status of your pods and services to ensure they are running correctly.
```bash
kubectl get pods
kubectl get services
```

#### 4. Access the service
Minikube provides a way to access services running inside the cluster using minikube service.
```bash
minikube service pos-service
```

You can also use `kubectl port-forward` to forward a port from your local machine to a port on a pod. For example:
```bash
kubectl port-forward deployment/pos-service 8080:8080
```