# UDP Chat Application (C#)

## Descriere

Această aplicație implementează un chat simplu utilizând protocolul **UDP**.

Proiectul conține două aplicații de consolă:

- **Server** – primește mesaje de la clienți și le retransmite către toți participanții activi
- **Client** – trimite și primește mesaje prin UDP

Aplicația permite comunicarea simultană între mai mulți clienți.

> Notă: UDP este connectionless, nu garantează livrarea sau ordinea mesajelor.

---

## Cum compilezi aplicația

### 1. Compilează Serverul
```bash
cd UdpChatServer
dotnet build
