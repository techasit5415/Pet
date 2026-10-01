# Desktop Pet — เคอร์บี้เดินบน Windows

ต้นแบบ C# WPF สำหรับ Windows 10/11 x64 พร้อมเคอร์บี้สไตล์พิกเซลอาร์ตตามภาพอ้างอิงที่ผู้ใช้ให้ วาดด้วยช่องพิกเซล WPF ไม่ต้องใช้ไฟล์ภาพหรือ NuGet เพิ่มเติม

## เริ่มใช้งาน

1. ติดตั้ง .NET 8 SDK สำหรับ Windows x64: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
2. แตก ZIP แล้วดับเบิลคลิก `run.bat` หรือเปิด terminal ในโฟลเดอร์แล้วใช้ `dotnet run`
3. สำหรับใช้ประจำ ให้เรียก `publish.bat` จากนั้นเปิด `publish/DesktopPet.exe` (เครื่องปลายทางไม่ต้องติดตั้ง .NET)
4. คลิกขวาที่ตัวเคอร์บี้เพื่อเปิดเมนู

## พฤติกรรม

- Idle → สุ่มเดินซ้าย/ขวา กระโดด นั่ง นอน หรือดูดลม; เดินมีเท้าสลับ ตัวเด้ง และกะพริบตา
- คลิกซ้ายแสดงหัวใจ; กดแล้วลากเกิน 5 pixels เพื่อลากตัวละคร; ปล่อยแล้วตกลงพื้น
- Follow mouse เปิด/ปิดผ่านคลิกขวาและจำค่าไว้ ตัวเคอร์บี้เดินตามแนวนอน ไม่บินตามตำแหน่ง Y ของเมาส์
- Pause, Jump, Sit, Sleep, Inhale (ท่าดูดลมปากกลม แก้มชมพู และประกายลม 4 วินาที), Move to next monitor, Size (เลือกขนาดตัวละคร: Tiny 28px, Small 56px, Medium 84px, Large 112px) และ Exit อยู่ในเมนู
- ข้ามจออัตโนมัติเมื่อจอวางติดกันในแนวนอน; จอแนวตั้งหรือมีช่องว่าง ใช้ลากหรือ Move to next monitor
- ขอบเขตใช้ work area ของแต่ละจอ เพื่อเว้น Taskbar ที่ Windows จองไว้ อัปเดตทุกวินาทีเมื่อเปลี่ยนจอ/Taskbar
- Start with Windows เป็นตัวเลือกในเมนู เปิดจาก EXE หลัง publish ก่อนใช้งาน ตัวเลือกนี้เพิ่ม/ลบเฉพาะค่า MarkDesktopPet ใน HKCU Run ของผู้ใช้ปัจจุบัน ไม่ต้องเป็น Administrator
- หน้าต่างโปร่งใส ไร้กรอบ อยู่ด้านบน ไม่แสดงใน Taskbar; คลิกได้เฉพาะรูปเคอร์บี้
- ป้องกันเปิดซ้ำสองตัวด้วย named mutex

## โครงสร้าง

- App.cs: จุดเริ่มต้นและ single instance
- Native.cs: Windows monitor, cursor และ window positioning API
- PetWindow.cs: state machine, physics, mouse, เมนู, startup, การปรับขนาด (Size)
- PetDrawing.cs: รูปและ animation ปรับตรงนี้เพื่อเปลี่ยนตัวละคร

## ข้อจำกัดและการตรวจสอบ

โปรเจกต์นี้สร้างบน Linux ที่ไม่มี .NET SDK จึงยังไม่ได้ compile หรือทดสอบ WPF บน Windows จริง ไม่ได้รวม EXE ที่อ้างว่าทดสอบแล้ว

ใช้ DPI แบบ system-aware เพื่อให้ตำแหน่งหลายจออยู่ในระบบพิกัดเดียวกัน จอที่ scale ต่างกันอาจทำให้รูปเบลอจาก Windows scaling ไม่ใช่ per-monitor DPI rendering
จอที่วางติดกันแต่ระดับพื้นต่างกันจะย้ายเคอร์บี้ขึ้น/ลงที่ขอบจอทันที การลากจะยึดตัวเคอร์บี้ให้อยู่ใน work area หนึ่งจอเสมอ
Taskbar แบบ auto-hide อาจซ้อนกับตัวเคอร์บี้ได้เมื่อปรากฏ เพราะ Windows ไม่จองพื้นที่ work area ในโหมดนั้น
Always-on-top อาจมองไม่เห็นในเกม fullscreen exclusive หรือ secure desktop (UAC)

เช็กบน Windows ก่อนใช้งานประจำ:

- `dotnet build` ผ่าน; `run.bat` เปิดหน้าต่างโปร่งใสและเดิน/กระโดดได้
- คลิกแล้วมีหัวใจ; ลากแล้วไม่ทะลุ Taskbar; ปล่อยแล้วลงพื้น; Pause หยุดและ Resume เดินต่อ
- ปรับขนาดผ่านคลิกขวา -> Size เลือกขนาด Small (56px), Medium (84px), Large (112px) หรือ Tiny (28px) ได้ทันที
- เปิด Follow mouse แล้วเดินตาม X; ทดสอบสองจอรวมจอที่มีพิกัดติดลบและ scale ต่างกัน
- ถอดจอแล้วเคอร์บี้กลับมาอยู่จอที่เหลือ; ย้าย Taskbar แล้วเคอร์บี้ยังอยู่ใน work area
- Publish แล้วเปิด EXE; เปิด startup และตรวจหลัง sign out/sign in; ปิด startup แล้วรายการหาย
- ปิดจาก Exit แล้ว process หาย; เปิดโปรแกรมซ้ำแล้วไม่สร้างเคอร์บี้เพิ่ม

อ้างอิง API: https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getmonitorinfoa

## ปรับหน้าตาเคอร์บี้

แก้ `PetDrawing.cs` เท่านั้น ระบบการเดิน/ลากยังอยู่ใน `PetWindow.cs`

- `Sprite` และ `Palette`: รูปพิกเซล สีชมพู เงา รองเท้า และดวงตาจากภาพอ้างอิง
- `PixelSize`: แต่ละ pixel เป็นสี่เหลี่ยมขนาด WPF units (เริ่มต้นที่ 2 = 56px สไตล์ Desktop Pet ทั่วไป หรือเลือกเปลี่ยนขนาดได้จากเมนูคลิกขวา)
- ตาราง `Sprite` ขนาด 19×16 ช่องเป็นท่าปกติ ส่วนท่ากระโดดใช้รูปพิกเซลแยกที่วาดตามภาพอ้างอิง และท่าอื่นปรับจากท่าปกติ
- `OnRender`: เลือกท่านอน นั่ง เดิน กระโดด reaction และลาก ตาม state
- `RenderOptions.SetEdgeMode(..., EdgeMode.Aliased)`: ให้ขอบพิกเซลคม

ภาพพิกเซลในโค้ดถอดรูปและสีจากภาพที่ผู้ใช้แนบมา พื้นสีดำภายนอกตัวละครโปร่งใสเมื่อแสดงบนเดสก์ท็อป
หลังแก้ไฟล์ ให้ปิดโปรแกรมแล้วเปิด run.bat ใหม่ หรือ publish.bat ใหม่หากใช้ EXE
