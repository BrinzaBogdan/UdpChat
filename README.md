# UDP Chat Application (C#)

## Descriere

Această aplicație implementează un chat simplu utilizând protocolul **UDP**.

Proiectul conține peer to peer method:

- **Client** – trimite și primește mesaje prin UDP

Aplicația permite comunicarea simultană între mai mulți clienți.

> Notă: UDP este connectionless, nu garantează livrarea sau ordinea mesajelor.

---

## Cum compilezi aplicația

### 1. Compilează Serverul
```bash
cd UdpChatServer
dotnet build
