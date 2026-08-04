![Bio Pest Control](frontend-web/src/assets/hero.png)

# Bio Pest Control System

BioPestControl is a high-performance, e-commerce platform specifically designed for stores trading in biological plant protection products (pesticides, organic fertilizers, etc.) in Vietnam. 
It aims to digitalize traditional business models, manage specialized product categories, and streamline complex operational processes to provide a greener and safer future for agriculture.

---

## 👥 User Roles & Features

The system implements strict Role-Based Access Control (RBAC) with four main roles:

### 1. Guest
Visitors who have not logged in can:
- **Browse & Search Catalog:** View product lists, filter by category/price, and search for specific bio-pesticides.
- **News & Community:** Read agricultural news, guidelines, and articles.
- **Contact:** Reach out to agricultural experts for support.

| Catalog Browsing | Agricultural News & CMS | Contact Us |
| :---: | :---: | :---: |
| ![Catalog](docs/assets/image35.png) | ![News](docs/assets/image23.png) | ![Contact](docs/assets/image11.png) |

---

### 2. Customer
Registered users can access a comprehensive shopping & support experience:
- **Authentication & Profile:** Sign in via email/Google, manage personal profile & security credentials.
- **Shopping Cart & Checkout:** Manage cart items, select shipping address, and proceed to checkout using multiple payment methods (COD or online via PayOS).
- **Order Management:** Track order history, delivery status, and order item details.
- **Agricultural Support Tools:** Access specialized tools like the Drug Dosage Calculator and Chemical Mixability Checker.

| Authentication (Sign In & Sign Up) |
| :---: |
| ![Login](docs/assets/image8.png) |
| ![Register](docs/assets/image39.png) |

| Shopping Cart & Checkout Flow |
| :---: |
| ![Cart](docs/assets/image20.png) |
| ![Shipping Address](docs/assets/image19.png) |
| ![Payment Methods](docs/assets/image21.png) |

| Order History & Order Details |
| :---: |
| ![Orders](docs/assets/image13.png) |
| ![Order Details](docs/assets/image37.png) |

| Agricultural Calculator & Mixability Checker |
| :---: |
| ![Agri Calculations](docs/assets/image12.png) |

---

### 3. Staff
Operational staff responsible for daily tasks:
- **Product & Inventory:** Update product info and manage warehouse manual/Excel imports and stock levels.
- **Order Processing:** Monitor and update customer order statuses.
- **Customer Interaction:** Moderate customer feedback and comments.

| Warehouse & Inventory Management |
| :---: |
| ![Warehouse Inventory](docs/assets/image16.png) |

---

### 4. Admin
System administrators with full access to reporting and system operations:
- **Business Dashboard:** View real-time statistics on revenue trends, total units sold, customer metrics, and top-selling products.
- **Catalog & Categories:** Manage product master data, categories, and chemical safety information.
- **Staff Management:** Manage staff accounts, permissions, and roles.

| Executive Business Dashboard |
| :---: |
| ![Admin Dashboard](docs/assets/image36.png) |

| Product Catalog & Category Management |
| :---: |
| ![Product Management](docs/assets/image10.png) |
| ![Category Management](docs/assets/image30.png) |

---

## 🏗 System Architecture

The platform is built on a scalable microservices architecture utilizing modern technologies (ASP.NET Core, SQL Server, MongoDB, Kafka, etc.):

- **Identity Service:** Authentication, JWT token issuance, and RBAC management.
- **Catalog Service:** Product catalog and master data.
- **Inventory Service:** Warehouse operations and stock levels.
- **Ordering Service:** Order lifecycle and checkout workflows.
- **Payment Service:** Third-party gateway integrations (PayOS, VNPay).
- **Agri Expert Service:** Agricultural data, dosage calculations, and chemical mixability checking.
- **Trading Service:** Specialized trading transactions and exchanges.
- **Article Service:** Content Management System (CMS) for news and agricultural guidelines.

### High-Level Microservices Architecture

![Microservices Architecture Diagram](docs/assets/image18.png)

```mermaid
graph TD
    Client([Client / Web User]) --> Gateway
    
    subgraph VM1 [VM 1: Gateway Node - 10.0.0.1]
        Gateway[Cloudflared / Traefik / Kong]
    end
    
    subgraph VM2 [VM 2: Worker Node - 10.0.0.2]
        K3S[K3s Cluster]
        Services[Core Microservices]
        K3S --- Services
    end
    
    subgraph VM3 [VM 3: Infra Node - 10.0.0.3]
        DB[(MongoDB)]
        Cache[(Redis)]
        MQ[[Kafka & Zookeeper]]
        Monitor[Prometheus & Grafana]
        Reg[Docker Registry]
    end
    
    Gateway --> K3S
    Services --> DB
    Services --> Cache
    Services --> MQ
```

### 🚀 Infrastructure & Deployment

The system is containerized and deployed across a cluster of 3 Virtual Machines (VMs) to ensure high availability, distributed processing, and seamless traffic routing:

#### VM 1: Gateway (10.0.0.1)
Acts as the entry point and ingress controller for external traffic.
- **Traefik & Kong Gateway:** API gateways and load balancers directing traffic to respective microservices.
- **Cloudflared Tunnel:** Secure tunneling for external access.

#### VM 2: Worker Node (10.0.0.2)
The primary computational node running the Kubernetes (K3s) environment.
- Hosts the core microservices (Identity, Catalog, Ordering, etc.).
- Serves as the K3s control-plane and worker node.

#### VM 3: Infrastructure (10.0.0.3)
Centralizes all stateful services, messaging queues, and system monitoring tools.
- **Message Broker:** Kafka & Zookeeper for asynchronous event-driven communication.
- **Databases & Cache:** MongoDB (for unstructured data) and Redis.
- **Monitoring & Logging:** Prometheus and Grafana for real-time system metrics and dashboards.
- **Registry:** Local Docker Registry for storing container images.

---

## 🗄 Database ERD Schemas

| User Accounts & Permission Management ERD | Cart & Order Systems ERD |
| :---: | :---: |
| ![User ERD](docs/assets/image26.png) | ![Order ERD](docs/assets/image29.png) |

---

## 🖼 Feature Screenshots Summary

Below is a gallery summarizing key system interfaces:

| Catalog & Products | Agricultural Calculations | Admin Dashboard |
| :---: | :---: | :---: |
| ![Catalog](docs/assets/image35.png) | ![Agri Calculations](docs/assets/image12.png) | ![Dashboard](docs/assets/image36.png) |

| Shopping Cart | Checkout Payment | Order Tracking |
| :---: | :---: | :---: |
| ![Cart](docs/assets/image20.png) | ![Payment](docs/assets/image21.png) | ![Orders](docs/assets/image13.png) |

| Login Interface | User Profile | Warehouse Imports |
| :---: | :---: | :---: |
| ![Login](docs/assets/image8.png) | ![Profile](docs/assets/image31.png) | ![Warehouse](docs/assets/image16.png) |

---

## 📖 Quick User Guide

### For Customers
1. **Registration:** Click `Login -> Create an Account`. Enter your details or register using Google.
2. **Shopping:** Browse products on the `Products` page. Click `Add to Cart` on your desired items.
3. **Checkout:** Go to your Cart, click `Proceed to Checkout`, enter shipping info, choose payment (COD/PayOS), and place the order.
4. **Agri Support:** Use the `Agricultural Calculator` to compute dosages and `Checker Mixability` before mixing chemicals.

### For Staff/Admin
1. Navigate to the specialized administration interface (Blazor/Razor Pages).
2. Use the left sidebar to manage `Customers`, `Orders`, `Warehouse`, and `Products`.
3. (Admin) Access the `Revenue Dashboard` to monitor overall business performance.

---
*Developed by Group 4 - FPT University*

