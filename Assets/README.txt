Asset Importer
__________________________________________________

BCG Asset Importer aracını "Tools" içerisinde bulabilirsiniz, script, Editor klasöründe yer almaktadır.

Aracı Unity 2022.3 ve Unity 6000.0 sürümlerinde; ayrıca Mac ve Windows cihazlarda test ettim.

Tasarımsal olarak değiştirilmesi gereken yerler olduğunu düşünsem de aslına sadık kalarak referans resimlerdeki gibi tasarladım.

İlk kez kullanacak kişiler için aydınlatıcı olması adına bilgi pencereleri ekledim.

İsimlendirme ya da seçilmeyen klasör yolları gibi durumlara karşı uyarı pencereleri ekledim.

Harici klasördeki dosyaları ReorderableList ile tutmayı tercih ettim.

ReorderableList'ten silinen harici klasör dosyası karşılaştırmadan çıkarılır, dolayısıyla kopyalanmaz.

Unity 6 öncesi sürümler emoji desteklemediği için EditorGUIUtility.IconContent içindeki ikonları kullandım.

Hedef klasörü ve içindekileri komple klonlamak yerine yeni isimde bir klasör oluşturup içindeki objeleri de isimlerini değiştirip bir listede string olarak tuttum, 
değiştirilmiş isimleri eşlesen dosyaları harici klasörden yeni oluşturulan klasöre kopyaladım 
(klonlanacak hedef klasörün yüzlerde objeli ağır bir klasör olabilme ihtimaline karşı dosya isimlerini liste olarak tutup karşılaştırdım)

Gördüğüm kadarıyla isimlendirmeler hep küçük harf ile yapılıyor ama gerekli olabilecek durumlara karşı isim değiştirme kısmına büyük-küçük harf duyarlılığı kontrolü ekledim.

Birden fazla ekranda çalışırken ya da yanlışlıkla yapılabilecek boyutlandırma hatalarından dolayı pencerenin kaybolması gibi hatalara karşı 
başlangıçta ekranın ortasında başlatmayı seçtim pencereyi.

Çeşitli notlarım script içinde bulunmakta.


UI-UX
__________________________________________________

Tek bir manager scripti ve bir adet Scriptable Object ile verileri tutup düzenledim.

Tüm cihazlarla uyumlu olabilmesi için basit bir SafeAreaAdjuster scripti oluşturdum.

Sol panel resimleri atanan aparata göre güncellenmekte.

Her bir kategori aynı alt panel ögelerini güncellemekte.

Sağ stats paneli maskeli ve kaydırılabilir şekilde. 

Silah üzerindeki aparatları bu projede hiyerarşiden aktif etmeyi seçtim.

Ön izleme ve seçim silah üzerinde güncellenir vaziyette ve seçim olmadan ön izlemeden başka kategoriye çıkılırsa silah üzerindeki aparat da kayıtlı olana dönmekte.

Sol ve alt panel seçili olan buton sarı çerçeveli olarak referanstaki gibi ayarlandı. Seçili olan aparat da otomatik olarak seçili geliyor.