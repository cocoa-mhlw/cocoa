---
title: "プライバシーポリシー"
weight: 10
type: docs
---

## 日本語

https://www.mhlw.go.jp/stf/seisakunitsuite/japanese_pp_00027.html

### 接触確認アプリケーションプライバシーポリシー

【2020年12月15日 プライバシーポリシー改定のお知らせ】

これまで、接触確認アプリは、陽性である旨の登録がなされると、14日間遡って接触の可能性のある利用者に通知をする仕組みとなっていました。厚生労働省は、保健所が行う積極的疫学調査との整合性を図る観点から、これを改め、陽性となった利用者が発症日又は検査日を接触確認アプリに入力し、その約２日前以降（感染可能期間内）に陽性者と接触の可能性のある利用者に通知をするよう、接触確認アプリの修正版の配布を開始しました。これに伴い、プライバシーポリシーを改定しています。主な変更は以下のとおりです。

 * 接触確認アプリの仕組みとして、陽性者が処理番号に加えて発症日又は検査日を入力し、その約２日前以降（感染可能期間内）に生成された日次鍵を通知サーバーに送信することにより、感染可能期間内にある陽性者との接触について通知する仕組みである旨を記載しました。
 * 厚生労働省が接触確認アプリを用いて取得する情報のうち、日次鍵については、感染可能期間内に生成されたもののみであることを明記しました。
 * 厚生労働省が、接触確認アプリに入力された発症日や検査日を取得したり、他のアプリ利用者に提供したりすることもないことを明記しました。


○接触確認アプリケーションプライバシーポリシー
 
厚生労働省は、接触確認アプリケーション（以下「本アプリ」といいます。）の提供に関し、適用ある法令を遵守するとともに、利用者のプライバシーの保護に最大限に配慮し、以下のポリシーにより、本アプリを提供します。
 
１ 本アプリの仕組み

① アプリ利用者（本アプリを利用して、本アプリが提供するサービスの利用を行う者をいいます。以下同じ。）のアプリ導入端末（本アプリを導入したスマートフォン端末をいいます。以下同じ。）において自動的に日次鍵（アプリ導入端末において、当該端末と一対一の対応関係を持ち、24時間単位で変更される識別子をいいます。以下同じ。）が生成され、記録されます。

② アプリ利用者のアプリ導入端末において接触符号（アプリ導入端末において、日次鍵をもとに生成され、10分単位で変更される識別子をいいます。以下同じ。）が自動的に生成され、記録されます。

③ アプリ利用者及び近接した状態にある他のアプリ利用者がそれぞれのアプリ導入端末のBluetoothを起動している間に限り、Bluetoothを利用して、(i)自らのアプリ導入端末において生成され記録されている接触符号が、近接した状態にある他のアプリ利用者のアプリ導入端末に自動的に提供され、記録されるとともに、(ii)当該他のアプリ利用者のアプリ導入端末において生成され記録されている接触符号が、自らのアプリ導入端末に対して自動的に提供され、記録されます。

④ アプリ利用者が、自らが陽性者（新型コロナウイルス感染症の陽性診断が確定した者をいいます。以下同じ。）であると判明した場合において、陽性者である旨をアプリにおいて登録する場合には、(i)(A)管理システム（新型コロナウイルスの陽性者及び濃厚接触者の情報を管理するため、厚生労働省が運用し、都道府県及び保健所設置市において利用される、新型コロナウイルス感染者等情報把握 * 管理支援システムをいいます。以下同じ。）に別途登録した自らの携帯電話番号又はメールアドレスに通知された処理番号（アプリ利用者が陽性者であると判明した場合に、管理システムから当該アプリ利用者に対して、ランダムに発行され、通知がされる無意かつ一時的な番号をいいます。以下同じ。）及び(B)当該陽性者に新型コロナウイルス感染症の症状がある場合は発症日、ない場合には検査日（発症日又は検査日の約２日前以降を「感染可能期間」とします。）を自らのアプリ導入端末に入力します。それにより、(ii)当該アプリ導入端末から通知サーバー（アプリ導入端末と連携して、アプリ利用者が必要事項に同意の上で端末から登録した日次鍵を管理し、一定の条件の下で当該日次鍵を他のアプリ利用者のアプリ導入端末に提供する機能を有する、厚生労働省が管理するサーバーをいいます。以下同じ。）を経由して管理システムに対し、入力された処理番号が陽性者に対して発行されたものであるか否かの照会が行われ、(iii)管理システムから通知サーバーに対し、当該照会された処理番号が陽性者に対して発行されたものであるか否かについての回答が行われます。

⑤ かかる照会の結果、当該照会された処理番号が陽性者に対して発行されたものである旨の回答が行われた場合は、陽性者自らのアプリ導入端末に記録された日次鍵（当該陽性者の感染可能期間内に生成されたものに限ります。）が、通知サーバーを経由して他のアプリ利用者のアプリ導入端末に自動的に提供され、当該他のアプリ利用者のアプリ導入端末において、最大で過去14日間分さかのぼって当該他のアプリ利用者のアプリ導入端末内に記録された接触符号の検索が自動的になされ、接触（概ね１メートル以内の距離で、15分以上の近接した状態をいいます。以下同じ。）にあたる接触符号の記録があることが判明した場合には、当該他のアプリ利用者のアプリ導入端末において、不特定の感染可能期間内にある陽性者との接触の可能性についての通知がされます。
 
２ 厚生労働省が本アプリを用いて取得する情報及び取得しない情報

(1) 厚生労働省が取得する情報
 * 厚生労働省は、本アプリを用いて、以下に掲げる情報を取得します。

① 上記１④のプロセスにおいて取得する陽性者の処理番号

② 上記１⑤のプロセスにおいて取得する陽性者の日次鍵（当該陽性者の感染可能期間内に生成されたものに限ります。）

③ 上記１の本アプリの改善のために必要な本アプリの動作情報（本アプリで実施した処理の内容、処理が行われた時刻、処理の成功/失敗、処理の実施にあたり参照した情報、処理の結果として出力した情報、実施時の状態等を含みますが、処理番号、日次鍵、接触符号を含みません。）及び本アプリの利用環境に関する情報（利用している本アプリのバージョン、利用端末のOS、OSバージョン、端末機種を指します。以下、動作情報とあわせて「動作情報等」といいます。）

(2) 厚生労働省が取得しない情報
 * 厚生労働省は、本アプリを用いて、(1)記載の情報以外の情報（以下に掲げる情報を含みますが、これらに限られません。）を取得しません。
 * 厚生労働省は、本アプリを用いて、アプリ利用者から、名前、生年月日、性別、住所、電話番号、メールアドレス、端末の位置情報その他のアプリ利用者を個人として識別可能な情報及びIPアドレス、MACアドレス、ホスト名その他の個別のアプリ導入端末を識別しうる情報を取得しません。
 * 接触符号は、アプリ利用者が保有する各々のアプリ導入端末内で暗号化した状態で記録され、アプリ利用者を含めいかなる者も把握することはできず、厚生労働省もその情報を取得しません。
 * 厚生労働省は、本アプリを用いて、陽性者を個人として識別可能な情報を取得しません。そのため、厚生労働省が、陽性者の同意のもと、過去14日以内における感染可能期間内にある当該陽性者との接触に関する情報について、他のアプリ利用者が本アプリによる通知を受け取る際に、当該通知を受ける者に対し、当該陽性者を個人として識別可能な情報を提供することもありません。また、厚生労働省が、１④(i)(B)で入力された発症日や検査日を取得したり、他のアプリ利用者に提供したりすることもありません。
 * 厚生労働省は、本アプリを用いて、感染可能期間内にある陽性者と接触の可能性がある旨の通知を受けた者について、個人として識別可能な情報を取得しません。そのため、厚生労働省が、本アプリを用いて、当該陽性者に対し、通知を受けた者を個人として識別可能な情報を提供することもありません。
 * 厚生労働省は、本アプリを用いて、感染可能期間内にある陽性者との接触の可能性がある旨の通知をうけた他のアプリ利用者と当該陽性者との間の対応関係や接触の日時に関する情報を取得しません。
 
３ 厚生労働省が本アプリを通じて取得する情報の利用目的及び利用方法

(1) 処理番号
 * 厚生労働省は、本アプリにおいて陽性者でない者が陽性である旨の登録をすることを避けるために、厚生労働省が取得した２(1)①記載の陽性者の処理番号を使用します。すなわち、陽性者であるアプリ利用者が本アプリにおいて自ら陽性である旨の登録をする際に管理システムから当該陽性者に対して処理番号を発行の上通知し、陽性者がアプリ導入端末で処理番号を入力することにより厚生労働省の通知サーバーが取得した処理番号は、通知サーバーから管理システムに対する、入力された処理番号が陽性者に対して発行されたものであるか否かの照会に使用され、当該処理番号が陽性者に対して発行されたものであることが確認されてはじめて陽性である旨の登録が完了するという仕組みをとっています。
 * 厚生労働省は、かかる目的以外の用途には、取得した処理番号を用いません。処理番号は、入力された処理番号が陽性者に対して発行されたものであるか否かの確認が完了した後、アプリ、通知サーバー及び管理システムのそれぞれにおいて、直ちに削除されます。

(2) 日次鍵
 * 厚生労働省は、14日以内に感染可能期間内にある陽性者であるアプリ利用者と接触状態となったことのある可能性のある他のアプリ利用者に対してその旨を通知するために、厚生労働省が取得した２(1)②記載の陽性者の日次鍵（当該陽性者の感染可能期間内に生成されたものに限ります。以下、本(2)において同じ。）を使用します。すなわち、陽性者であるアプリ利用者の陽性登録完了により厚生労働省の通知サーバーが取得した陽性者の日次鍵が他のアプリ利用者のアプリ導入端末に提供され、当該端末内に記録された接触符号が自動的に検索された結果、接触にあたる接触符号があった場合に、当該他のアプリ利用者に対して感染可能期間内にある陽性者との接触可能性についての通知がなされるという仕組みをとっています。
 * 厚生労働省は、陽性者であるアプリ利用者が陽性である旨の登録を希望する場合は、あらかじめ、(a)他のアプリ利用者のアプリ導入端末に自らのアプリ導入端末に記録された日次鍵が提供され、かつ、(b)他のアプリ利用者のうち感染可能期間内に自らと接触状態となったことのある者については当該陽性者を個人として識別可能な情報の提供を受けずに不特定の感染可能期間内にある陽性者との接触した可能性がある旨を知ることができる状態となることについて、本アプリ上で改めて明示的な同意を取得します。
 * 厚生労働省は、かかる目的以外の用途には、取得した日次鍵を用いません。全ての日次鍵（当該陽性者の感染可能期間外に生成されたものを含みます。）は、各アプリ導入端末において生成されてから14日が経過した後に自動的に無効となります。

(3) 動作情報等
 * 厚生労働省は、アプリ利用者から寄せられる本アプリの障害の可能性等に関する情報を元に、本アプリの改善のために必要な対処を速やかに行うため、厚生労働省が取得した２(1)③記載の動作情報等を使用します。
 * 厚生労働省は、アプリ利用者が本アプリの障害の可能性等を感じ、任意で動作情報等を送信しない限り、動作情報等を取得することはありません。
 * 動作情報等を送信したアプリ利用者が本アプリの障害等の調査にさらに任意でご協力いただける場合には、メールを通じて、６記載のサポートデスク宛に、動作情報ID（動作情報等に対して振り出されるランダムな番号で、問い合わせ事象と動作情報等の紐付けのみに利用する符号のことをいいます。）を伝達いただきます。厚生労働省は、動作情報IDを利用して、サポートデスクで受け付けた問い合わせ内容と動作情報等を紐付けて管理します。厚生労働省は、かかる情報管理を行うことについて、本アプリ上であらかじめ明示的な同意を取得します。
 * 厚生労働省は、かかる目的以外の用途には、動作情報等を用いません。動作情報等は、当該動作情報等に係る障害調査終了後、サーバーにおいて直ちに削除されます。


 
４　同意の撤回と記録の削除
 * 本アプリの利用に関する同意は、アプリ導入端末から本アプリを削除する方法によりいつでも撤回できます。アプリ利用者が上記の方法により同意を撤回した場合は、アプリ利用者のアプリ導入端末内に記録された全ての情報は、削除され、復元できなくなります。
 * アプリ導入端末に記録された他のアプリ利用者との近接に関する情報（他のアプリ利用者のアプリ導入端末に記録された接触符号）は、暗号化されて記録され、14日の経過後に、自動的に無効となります。
 * アプリ導入端末に記録された動作情報は、生成から14日の経過後に、自動的に削除されます。
 
５　アプリ利用者の情報の管理
 * それぞれの導入端末の近接に関する情報は、あくまでそれぞれの導入端末内で管理され、導入端末から外部には提供されません。
 * 厚生労働省は、アプリ利用者のプライバシーの確保に支障が生じないよう、本アプリのシステムの運用において、不正アクセス、ウイルス * マルウェア等に対する適正な情報セキュリティ対策を講じます。本アプリのシステムの運用の一部を委託する場合には、当該委託先に対しても、適正な情報セキュリティ対策を講じさせます。
 * 厚生労働省は、本アプリの運用において、３(3)記載のアプリ利用者の明示的な同意がある場合を除き、本アプリ以外のシステム等（管理システムを含みますが、これに限られません。）を通じて国又は地方公共団体が管理する特定の個人を識別可能な情報と照合することによりアプリ利用者個人の識別につながることがないよう、取り扱うデータに関する適正な安全管理措置を講じます。
 
６　サポートデスク及びコールセンター
 * 厚生労働省は、本アプリに関する利用者等からのお問い合わせ * ご相談を受け付けるため、メールによるサポートデスク及び電話によるコールセンターを設置します。
 * 厚生労働省は、サポートデスク及びコールセンターで取得したメールアドレス又は電話番号を含む情報を、利用者からのお問い合わせ * ご相談にお答えするため（当該お問い合わせ * ご相談に関する障害等の調査のための利用を含みます。）のみに使用します。
 
７　第三者提供
 * 厚生労働省は、本アプリを通じて厚生労働省が取得した情報並びにサポートデスク及びコールセンターで厚生労働省が取得した情報を、本アプリ利用者の明示的な同意を得ることなく、第三者に提供しません。
 
８　業務委託
 * 厚生労働省は、本アプリ並びにサポートデスク及びコールセンターの運用の全部又は一部を業務委託（厚生労働省からの直接の業務委託だけでなく、当該業務委託に係る再委託及び再々委託も含みます。）し、当該委託先において本アプリを通じて取得する情報もしくはサポートデスク又はコールセンターで取得した情報を取り扱わせる場合があります。この場合には、当該委託先に対しても、適正な安全管理措置を講じさせるよう、管理 * 監督を行います。
 * 厚生労働省との業務委託関係にあり、本アプリを通じて取得する情報もしくはサポートデスク又はコールセンターで取得した情報を取り扱う委託先については、厚生労働省の接触確認アプリに関するホームページにてご案内します。
 
９　プライバシーに関するお問い合わせ先
 * 本アプリの利用におけるプライバシーに関するご質問等については、本アプリ内又は厚生労働省の接触確認アプリに関するホームページ内に掲載し、厚生労働省が指定するお問い合わせ窓口までお問い合わせください。

----

## English（英語）

https://www.mhlw.go.jp/stf/seisakunitsuite/english_pp_00032.html

### Contact Confirmation Application Privacy Policy

Notification of a Revision to the Privacy Policy on December 15, 2020

The contact confirmation application has until now employed a system that notifies users who may have been in contact with a person who has tested positive for COVID-19 over the past 14 days when a user registers that he/she has tested positive. The Ministry of Health, Labour and Welfare has now revised this from the perspective of ensuring consistency with the proactive epidemiological investigations conducted by health care centers. Therefore, we have started distributing a revised version of the contact confirmation application. In this version, a user who has tested positive for COVID-19 inputs the date of the onset of symptoms or the test date. The application then notifies users who may have been in contact with the said user who has tested positive for COVID-19 from approximately two days before that onward (within the infectable period). Accordingly, we have revised the privacy policy in line with this. The main changes are as follows.

We have stated that there is a system which notifies users that they may have been in contact with a person who has tested positive for COVID-19 within the infectable period as the system of the contact confirmation application. We have stated that this system works by the person who has tested positive for COVID-19 inputting the date of the onset of symptoms or the test date in addition to the processing number and then sending the daily key generated from approximately two days before that onward (within the infectable period) to the notification server.

We have specified that this only applies to the daily key generated within the infectable period in the information obtained by the Ministry of Health, Labour and Welfare using the contact confirmation application.
We have specified that the Ministry of Health, Labour and Welfare will not obtain or provide to other application users the date of the onset of symptoms or the test date input in the contact confirmation application.
 
 
○ Contact Confirmation Application Privacy Policy
 
The Ministry of Health, Labour and Welfare is committed to complying with all applicable laws and regulations, and giving maximum consideration to users’ privacy, in the provision of the Contact Confirmation Application (“the App”). To that end, the App is provided in accordance with the following Privacy Policy.
 
1 System of the App

A. In an App-Installed Device (a smartphone device in which the App is installed; the same applies hereinafter) of an App User (a person who uses a service provided by the App by using the App; the same applies hereinafter), a Daily Key (an identifier which, in the App-Installed Device, is unique to that Device and which is changed every 24 hours; the same applies hereinafter) is automatically generated and recorded.

B. In the App User’s App-Installed Device, a Contact Code (an identifier which is generated in an App-Installed Device on the basis of the Daily Key and which is changed every 10 minutes; the same applies hereinafter) is automatically generated and recorded.

C. Only when Bluetooth is enabled in the App-Installed Devices of the App User and another App User who is in a state of proximity with said App User, using Bluetooth, (i) the Contact Code generated and recorded in the App User’s own App-Installed Device is automatically provided to and recorded in the App-Installed Device of the other App User in a state of proximity with said App User, and (ii) the Contact Code generated and recorded in said other App User’s App-Installed Device is automatically provided to and recorded in the App User’s own App-Installed Device.

D. In the event that the App User has determined that they are a Positive Tester (a person who has tested positive for COVID-19; the same applies hereinafter), when said App User registers that they are a Positive Tester in the App, (i) said App User inputs, to their own App-Installed Device, (A) a Processing Number (a temporary number without any special meaning, which is issued randomly and communicated to an App User from a Management System in the event that the App User has been determined to be a Positive Tester; the same applies hereinafter) sent to said App User’s mobile telephone number or email address, which has been registered separately with the Management System (a COVID-19 infected person information identification and management support system, operated by the Ministry of Health, Labour and Welfare and used by prefectures and cities where public health centers are located, for the purpose of managing information of Positive Testers for COVID-19 and people who have been in close contact with Positive Testers; the same applies hereinafter), and (B) the date of the onset of symptoms in the event the said Positive Tester has symptoms of COVID-19 or the test date in the event the said Positive Tester has no symptoms of COVID-19 (hereinafter the period from approximately two days before the date of the onset of symptoms or the test date onward shall be referred to as the “Infectable Period”), (ii) whether or not the input Processing Number is a Processing Number issued to a Positive Tester is verified with the Management System from said App User’s App-Installed Device via a Notification Server (a server, managed by the Ministry of Health, Labour and Welfare, which is linked to the App-Installed Device and which has a function for, when the App User has accepted necessary matters, managing the Daily Key registered from the App-Installed Device and providing said Daily Key to the App-Installed Device of another App User under a certain condition or conditions; the same applies hereinafter), and (iii) a response indicating whether or not the verified Processing Number is a Processing Number issued to a Positive Tester is provided to the Notification Server from the Management System.

E. In the event where, as a result of said verification, a response indicating that the verified Processing Number is a Processing Number issued to a Positive Tester has been made, the Daily Key recorded in the App-Installed Device of the Positive Tester (only those generated within the Infectable Period of the said Positive Tester) is automatically provided to the App-Installed Device of said other App User via the Notification Server. In the App-Installed Device of said other App User, Contact Codes recorded in the App-Installed Device of said other App User within a maximum of the last 14 days are searched, and in the event where it is determined that a Contact Code equivalent to Contact (a state within which a person has been approximately 1 meter of another person for no less than 15 minutes; the same applies hereinafter) is recorded, a notification indicating that said other App User may have been in contact with an unspecified Positive Tester within the Infectable Period is made in the App-Installed Device of said other App User.

2 Information which is and is not collected by the Ministry of Health, Labour and Welfare using the App

(1) Information which the Ministry of Health, Labour and Welfare collects
 * The Ministry of Health, Labour and Welfare may use the App to collect the following information.

A. The Processing Number of the Positive Tester in the process of 1-D described above.

B. The Daily Key of the Positive Tester in the process of 1-E described above. (only those generated within the Infectable Period of the said Positive Tester).

C. The operating information of the App necessary to improve the App in 1 above (including details of the processing performed by the App, the time the processing was performed, whether the processing was a success or failure, information referred to when performing processing, information output as results of processing and the state at the time of processing etc., but not including the processing number, Daily Key or Contact Code) and information relating to the usage environment of the App (this refers to the version of the App being used, the OS of the usage device, the OS version and the device model; hereinafter, together with the operating information, referred to as “Operating Information etc.”).

(2) Information which the Ministry of Health, Labour and Welfare does not collect
 * The Ministry of Health, Labour and Welfare does not use the App to collect information aside from the information listed in (1) above (including, but not limited to, the information listed below).
 * The Ministry of Health, Labour and Welfare does not use the App to collect the App User’s name, date of birth, gender, address, telephone number, email address, device location information, or any other information which enables the App User to be personally identified, and the IP address, MAC address, host name or other information which may identify an individual App-Installed Device.
 * Contact Codes are recorded in an encrypted state within each App User’s App-Installed Device. Contact Codes cannot be known by any parties, including App Users, and the Ministry of Health, Labour and Welfare does not collect this information.
 * The Ministry of Health, Labour and Welfare does not use the App to collect information which enables a Positive Tester to be personally identified. As such, when, with the consent of a Positive Tester, another App User receives, through the App, a notification of information pertaining to contact with said Positive Tester within the Infectable Period which has occurred in the previous 14 days, the Ministry of Health, Labour and Welfare does not provide, to the person receiving said notification, any information which enables said Positive Tester to be personally identified. In addition, the Ministry of Health, Labour and Welfare does not obtain or provide to other App Users the date of the onset of symptoms or the test date input in 1 D (i) (B).
 * The Ministry of Health, Labour and Welfare does not use the App to collect information which enables a person who has received a notification that they may have come into contact with a Positive Tester within the Infectable Period to be personally identified. As such, the Ministry of Health, Labour and Welfare does not use the App to provide, to said Positive Tester, information which enables the person who has received the notification to be personally identified.
 * The Ministry of Health, Labour and Welfare does not use the App to collect information pertaining to relationships, dates and/or times of Contact, etc., between a Positive Tester and another App User who has received a notification that they may have come into contact with the Positive Tester within the Infectable Period.
 
3 Purposes for and methods through which Ministry of Health, Labour and Welfare uses information collected via the App

(1) Processing Number
 * The Ministry of Health, Labour and Welfare uses the Processing Number of the Positive Tester, described in 2(1)A and obtained by the Ministry of Health, Labour and Welfare, to ensure that a person who uses the App but is not a Positive Tester is not registered as having tested positive. In other words, the system is such that a Processing Number, which has been obtained from the Notification Server of the Ministry of Health, Labour and Welfare by an App User who is a Positive Tester inputting the Processing Number in their App-Installed Device after the Processing Number has been issued to said Positive Tester from the Management System when said Positive Tester registers, in the App, that they are a Positive Tester, is used to verify whether or not the input Processing Number is a Processing Number that has been issued to a Positive Tester, and the Positive Tester is only registered as having tested positive upon it being confirmed that said Processing Number is a Processing Number that has been issued to a Positive Tester.
 * The Ministry of Health, Labour and Welfare will not use the obtained Processing Number for any purposes other than the stated purpose. The input Processing Number is deleted from the App, the Notification Server, and the Management System immediately upon completion of the confirmation as to whether or not the Processing Number has been issued for a Positive Tester.

(2) Daily Key
 * The Ministry of Health, Labour and Welfare uses the Daily Key of the Positive Tester, described in 2(1)B and obtained by the Ministry of Health, Labour and Welfare (only those generated within the Infectable Period of the said Positive Tester; hereinafter the same in this section (2)), to notify another App User that they may have been in a state of Contact with an App User who is a Positive Tester within the Infectable Period in the previous 14 days. In other words, the system is such that the Daily Key of an App User who is a Positive Tester is obtained from the Notification Server of the Ministry of Health, Labour and Welfare upon said Positive Tester being registered as having tested positive and provided to the App-Installed Device of another App User, and in the event where an automatic search of Contact Codes recorded in said terminal has returned a Contact Code equivalent to Contact, a notification of potential Contact with a Positive Tester within the Infectable Period is provided to said other App User.
 * In the event where an App User who is a Positive Tester desires to be registered as having tested positive, the Ministry of Health, Labour and Welfare will, in advance and through the App, obtain explicit consent to (a) the Daily Key recorded in said App User’s own App-Installed Device being provided to another App User’s App-Installed Device and (b) another App User who has been in a state of Contact with said App User within the Infectable Period being able to know that said other App User may have been in Contact with an unspecified Positive Tester within the Infectable Period without receiving information which enables said Positive Tester to be personally identified.
 * The Ministry of Health, Labour and Welfare will not use the obtained Daily Key for any purposes other than the stated purpose. All Daily Keys (including those generated outside the Infectable Period of the said Positive Tester) are automatically invalidated 14 days after being generated in the App-Installed Device. (3) Operating Information etc.
 * The Ministry of Health, Labour and Welfare will use the Operating Information etc. described in 2 (1) C that has been obtained by the Ministry of Health, Labour and Welfare to promptly take the necessary measures to improve the App based on information relating to the possibility of a malfunction in the App etc. sent by the App User.
 * The Ministry of Health, Labour and Welfare will not obtain Operating Information etc. unless the App User feels there is a possibility of a malfunction in the App etc. and then voluntarily sends Operating Information etc.
 * In the event an App User who has sent Operating Information etc. further voluntarily cooperates with an investigation into the malfunction in the App etc., the operating information ID (the operating information ID is a random number assigned to the Operating Information etc. and refers to a code used only to link an inquiry to Operating Information etc.) will be conveyed to the support desk described in 6 via e-mail. The Ministry of Health, Labour and Welfare will use the operating information ID to link and then manage inquiries received at the support desk and Operating Information etc. The Ministry of Health, Labour and Welfare will obtain explicit consent in advance in the App to perform such information management.
 * The Ministry of Health, Labour and Welfare will not use Operating Information etc. for any other purposes. The Operating Information etc. will be immediately deleted from the server upon the completion of the investigation into the malfunction that pertains to the said Operating Information etc.

4 Retraction of Consent and Deletion of Records
 * Consent pertaining to the use of the App can be retracted at any time by deleting the App from the App-Installed Device. In the event that the App User has retracted consent through the stated method, all information of the App recorded in the App User’s App-Installed Device will be irrecoverably deleted.
 * Information pertaining to proximity with other App Users recorded in the App-Installed Device (e.g., Contact Codes recorded in the App-Installed Devices of other App Users) are recorded in an encrypted state and are automatically invalidated after 14 days.
 * The operating information recorded in App-installed Devices will be automatically deleted 14 days after its generation.
 
5 Management of App User’s Information
 * Information pertaining to proximity in each App-Installed Device is essentially managed within the App-Installed Device, and is not provided to the exterior from the App-Installed Device.
 * In operating the system(s) for the App, the Ministry of Health, Labour and Welfare takes appropriate information security measures against improper access, viruses, malware, etc., to prevent the occurrence of any problems in ensuring the privacy of App Users. In the event that the system(s) for operating the App are partially contracted to another organization, that organization shall be required to take appropriate information security measures.
 * In operating the system(s) of the App, the Ministry of Health, Labour and Welfare will take appropriate measures for safely managing the data it handles so that App Users are not personally identified as a result of verification against information, managed by the national or local governments, which enables specific individuals to be identified, through system(s), etc. aside from those of the App (including, but not limited to, the Management System) unless in the event it has the explicit consent of the App User described in 3 (3).
 
6 Support Desk and Call Center
 * The Ministry of Health, Labour and Welfare will establish a support desk to receive inquiries and questions relating to the App from the App Users etc. by e-mail and a call center to do the same by telephone.
 * The Ministry of Health, Labour and Welfare will use information, including e-mail address and telephone numbers obtained at the support desk and call center, only to answer inquiries and questions from App Users (this includes use to investigate malfunctions etc. relating to the said inquiries and questions).
 
7 Third Party Provision
 * The Ministry of Health, Labour and Welfare will not provide information obtained by the Ministry of Health, Labour and Welfare via the App and information obtained by the Ministry of Health, Labour and Welfare at the support desk and call center to third parties without the explicit consent of the App User.
 
8 Outsourcing
 * The Ministry of Health, Labour and Welfare may outsource part or all of the operation of the App, support desk and call center (this includes not only direct outsourcing by the Ministry of Health, Labour and Welfare, but also first tier and second tier subcontracting pertaining to the said outsourcing) and the said subcontractor may then handle the information obtained via the App or the information obtained at the support desk or call center. In this case, the Ministry of Health, Labour and Welfare will manage and supervise the said subcontractor so that it takes appropriate safe management measures.
 * The Ministry of Health, Labour and Welfare will give information on the Ministry of Health, Labour and Welfare’s website concerning the contact confirmation application about subcontractors who have a subcontracting relationship with the Ministry of Health, Labour and Welfare and who handle information obtained via the App and information obtained at the support desk or call center.
 
9 Inquiries Regarding Privacy
 * Please contact the Ministry of Health, Labour and Welfare as specified in the App or on the Ministry of Health, Labour and Welfare’s website concerning the contact confirmation application for any questions, etc. regarding privacy in the use of the App.

----

## 簡体中国語

https://www.mhlw.go.jp/stf/seisakunitsuite/chinese_pp_00030.html

### 接触确认应用程序隐私保护方针

【2020年12月15日 隐私保护方针修订通知】

接触确认APP以往采取的机制是：如某位使用者登记为阳性者，则通知过去14天内可能与其有过接触的其他使用者。为了配合保健所正在积极开展的流行病学调查，厚生劳动省从与流行病学调查保持吻合性的视角出发，开始发布APP修正版，改为由确认为阳性的使用者将其发病日或检查日输入到接触确认APP中，并通知在改日期的约2天前以后（感染可能期间内）有可能与该阳性者有过接触的使用者。为此，运营方对隐私保护方针做了修订。主要的变更点如下。
 * 明确注明，接触确认APP的运行机制改为：阳性者需要在受理编号的基础上输入发病日或检查日，并向通知服务器发送其约2天前以后（感染可能期间内）所生成的日更密钥，从而通知在感染可能期间内与该阳性者有过接触的APP使用者。
 * 明确注明，在厚生劳动省使用接触确认APP所获取的信息之中，只包括在感染可能期间内所生成的日更密钥。
 * 厚生劳动省还明确表示，不会获取阳性者在接触确认APP中输入的发病日或检查日，并将其提供给其他的APP使用者。
 
○接触确认应用程序隐私保护方针

厚生劳动省在提供接触确认应用程序（以下简称“本APP”。）的过程中，应遵守其所适用的法律法规，最大限度地保护使用者的隐私，依据如下保护方针提供服务。

 
１ 本APP的运行机制

① 在APP使用者（是指通过本APP使用本APP所提供之服务者。下同。）的已安装APP的终端（是指安装有本APP的智能手机终端。下同。）上自动生成并记录日更密匙（与该终端具有一对一的对应关系，在已安装APP的终端中以24小时为单位变更的标识符。下同。）。

② 在APP使用者的已安装APP的终端上自动生成并记录接触符号（基于日更密匙所生成的，在已安装APP的终端中以10分钟为单位变更的标识符。下同。）。

③ APP使用者以及处于接近（在大约１米以内的距离保持15分以上的近距离接触的可能性较高的状态。下同。）状态的其他APP使用者各自启动其已安装APP的终端的蓝牙功能时，则使用蓝牙功能，(i)将自身的已安装APP的终端上所生成并记录的接触符号，自动提供给处于接近状态下的其他APP使用者的已安装APP的终端，并进行记录，(ii)将该其他APP使用者的已安装APP的终端上所生成并记录的接触符号自动提供至自身的已安装APP的终端中，并进行记录。

④ APP使用者在查明自身为阳性者（确诊新型冠状病毒感染为阳性者。下同。）的情况下，如希望在APP中登记为阳性者，则执行如下事项：(i)将（A）在管理系统上（为了对新型冠状病毒的阳性者以及密切接触者的信息进行管理，而由厚生劳动省进行运营，用于都道府县以及设置保健所的市，对于新型冠状病毒感染者等信息进行掌握和管理的支援系统下同。）另行登录的自身手机号码或邮箱地址所接收到的受理编号（在查明某位APP使用者为阳性者的情况下，由管理系统向该APP使用者随机发行，进行通知的任意临时编号。下同。）以及(B)该阳性者有新冠病毒感染症状的情况下则将其发病日，没有新冠病毒感染症状的情况下则将其检查日（发病日或检查日的约2天前以后称为“感染可能期间”。）输入到自身的已安装APP的终端，从而(ii)该已安装APP的终端经由通知服务器（由厚生劳动省进行管理的具有如下功能的服务器：与已安装APP的终端进行联动，让APP使用者可以在同意必要事项的基础上对从终端上登录的日更密匙进行管理，并在一定的条件的下将该日更密匙提供给其他APP使用者的已安装APP的终端。下同。）前往管理系统查询，确认所输入的是否确为发行给该阳性者的受理编号，然后(iii)管理系统回答通知服务器，所查询的受理编号是否确为发行给该阳性者的受理编号。

⑤ 相关查询的结果，如管理系统回答所查询的受理编号确为发行给该阳性者的受理编号，则记录于该阳性者自身的已安装APP的终端上的日更密匙（仅限在该阳性者的感染可能期间内所生成的日更密匙。）可以经由通知服务器自动提供至其他使用者的已安装APP的终端上，该其他使用者在其已安装APP的终端上最多可自动追溯搜索过去14天内该其他使用者的已安装APP的终端内所记录的接触符号，如查明存在符合接触状态（在大约１米以内的距离保持15分以上的接近状态。下同。）的接触符号记录，则该其他使用者的已安装APP的终端会收到可能与不特定的处于感染可能期间内的阳性者有过接触的通知。
 
２ 厚生劳动省通过本APP会获取以及不会获取的信息

(1)厚生劳动省会获取的信息
 * 厚生劳动省会通过本APP获取如下所示的信息。

① 在上述１④的流程中所获取的阳性者的受理编号

② 在上述１⑤的流程中所获取的阳性者的日更密匙（仅限在该阳性者的感染可能期间内所生成的日更密匙。）

③ 上述1中的改善本APP所需的程序运行信息（包括本APP所受理的内容、受理时间、受理是否成功、受理过程中所参考的信息、作为受理结果的输出信息、受理时的状态，但是不包括受理编号、日更密匙和接触符号。）以及本APP的运行环境相关信息（包括COCOA APP的版本、何种OS、OS的版本、设备机种，以下与APP运行信息并称“运行信息”等。）
 
(2) 厚生劳动省不会获取的信息
 * 厚生劳动省不会通过本APP获取如下所示的信息：(1)所记载的信息之外的信息（包括但不限于如下所示的信息。）。
 * 厚生劳动省不会通过本APP从APP使用者处获取其姓名、出生年月日、性别、住址、电话号码、邮箱地址、终端位置信息等可识别APP使用者个人身份的信息，以及能够识别IP地址、MAC地址、主机名以及其他的可识别安装APP的个别终端的信息。
 
 * 接触符号将分别在APP使用者各自持有的已安装APP的终端内以加密的状态进行记录，包括APP使用者在内的任何人都无从得知已安装APP的终端之间的接触性格信息，厚生劳动省也不会获取该信息。
 * 厚生劳动省不会通过本APP获取可识别阳性者个人身份的信息。为此，厚生劳动省在征得阳性者同意的基础上获取与该阳性者之间过去14天内与处于感染可能期间内的该阳性者之间的接触的相关信息后，其他APP使用者通过本APP接收通知时，不会向该接收通知者提供可识别该阳性者个人身份的信息。此外，厚生劳动省也不会获取１④(i)(B)中所输入的发病日或检查日，并将其提供给其他的APP使用者。
 * 对于通过本APP接到通知可能与处于感染可能期间内的阳性者有过接触者，厚生劳动省也不会获取可识别其个人身份的信息。为此，厚生劳动省也不会通过本APP向该阳性者提供可识别接收通知者个人身份的信息。
 * 厚生劳动省不会通过本APP获取接到可能与处于感染可能期间内的阳性者有过接触的通知的其他APP使用者与该阳性者之间的对应关系以及接触日期等相关信息。
 
３ 厚生劳动省通过本APP所获取的信息的使用目的以及使用方法

(1) 受理编号
 * 厚生劳动省为避免非阳性者在本APP中谎称阳性，将会使用在２(1)①中所获取的阳性者受理编号。换言之，所采用的运行机制如下：新型冠状病毒为阳性的APP使用者在本APP中将其自身登记为阳性时，管理系统会向该阳性者发行受理编号并进行通知，阳性者在已安装APP的终端上输入受理编号后，厚生劳动省的通知服务器所获取的受理编号将用于通知服务器向管理系统查询所输入的受理编号是否为向该阳性者发行的受理编号，确认该受理编号确为向该阳性者发行之后方可完成阳性者登记。
 * 厚生劳动省不会将所获取的受理编号用于原本目的之外的其他用途。在确认所输入受理编号是否为向该阳性者发行的受理编号之后，将立即在APP、通知服务器以及管理系统中分别删除该受理编号。

(2) 日更密匙
 * 为了通知其他APP使用者其可能在14天内与处于感染可能期间内的阳性APP使用者有过接触，厚生劳动省会使用其所获取的２(1)②中所记载的阳性者的日更密匙（仅限在该阳性者的感染可能期间内所生成的日更密匙。在本(2)中下同。）。换言之，采用如下的运行机制：阳性APP使用者的阳性登记完成之后，厚生劳动省的通知服务器所获取的该阳性者的日更密匙将会提供至其他APP使用者的已安装APP的终端中，该终端内所记录的接触符号自动检索的结果，如存在符合接触状态的接触符号，该其他APP使用者会接到可能与处于感染可能期间内的阳性者有过接触的通知。
 * 在阳性APP使用者希望登记为阳性的情况下，厚生劳动省将事先在本APP上针对如下事项征得其明确的同意如下事项：(a)向其他APP使用者的已安装APP的终端提供记录于自身的已安装APP的终端中的日更密匙，并且，(b)如其他APP使用者在感染可能期间内与自身有过接触状态，则在不提供可识别该阳性者个人身份的信息的前提下，通知对方其可能与不特定的处于感染可能期间内的阳性者有过接触。
 * 厚生劳动省所获取的日更密匙用于原有目的之外不用于其他用途。所有的日更密匙（包括该阳性者在其感染可能期间之外所生成的日更密匙。）在其生成于已安装APP的终端的14天后自动失效。
 
(3) APP运行信息等
 * 为了以APP使用者所传送的本APP故障可能性等的相关信息为基础，迅速对本APP进行改善，厚生劳动省会用到2（1）③中所记载的APP运行信息等。
 * 除非APP使用者感觉到本APP可能存在故障，并主动传送APP运行信息等，否则厚生劳动省不会获取APP运行信息等。
 * 如果已经传送了APP运行信息等的APP使用者还希望进一步自发性地协助本APP的故障调查，则可以通过电子邮件，向6中所记载的客户服务台发送APP运行信息ID（针对APP运行信息等而生成的随机号码，用于与所咨询事宜以及APP运行信息等建立关联的符号。）。厚生劳动省会利用APP运行信息ID，将在邮件服务台所受理的咨询内容和APP运行信息等进行关联管理。在进行信息管理时，厚生劳动省会在本APP上预先征得使用者的明确同意。
 * 厚生劳动省不会将APP运行信息等用于原本使用目的之外的其他用途。在该APP运行信息等所涉及的故障调查结束后，会立即从服务器上删除APP运行信息等。
 
４ 同意的撤回与记录的删除
 * 本APP使用的相关同意，可以通过在已安装APP的终端中删除本APP的方式进行撤回。APP使用者通过上述的方法撤回同意的情况下，则记录已安装APP的终端内的所有的APP使用者相关信息也随之全部删除，无法复原。
 * 进行加密，并在14天后自动失效。
记录于安装APP的终端中的运行信息，将在其生成的14天后自动删除。

 
５ APP使用者信息的管理
 * 各安装终端的接近相关信息，仅在该安装终端内进行管理，不会向安装终端的外部进行提供。
 * 在本APP的系统的运营过程中，除3（3）中所记载的APP使用者明确同意的情况外，厚生劳动省将会对所处理的数据采取适宜的信息安全对策，不会通过本APP以外的系统（包括但不限于管理系统）去确认、校验国家或地方公共团体所管理的能识别特定个人的信息，并由此去识别APP使用者的个人身份。
 * 为了通过本APP之外的其他系统等（包括但不限于管理系统。）与日本国家政府或地方公共团体所管理的可识别个人身份的信息不进行查询比较，来识别APP使用者的个人身份，厚生劳动省在本APP的系统运营中，将针对所处理的数据采取适宜的安全管理措施。将本APP的系统运营部分外包时，也会要求承包方采取适宜的安全管理措施。
 
６  邮件服务台和呼叫中心
 * 为了受理涉及本APP的使用者咨询和协商，厚生劳动省设置了邮件服务台和呼叫中心。
 * 对于通过邮件服务台和呼叫中心所获取的包含邮件地址和电话号码在内的的信息，厚生劳动省仅将其用于回答使用者的咨询或协商（包括调查该咨询或协商所涉及的相关故障等。）。
 
７ 向第三方提供
 * 在未经本APP使用者明确同意的情况下，厚生劳动省不会向第三方提供通过本APP所获得的信息以及通过邮件服务台和呼叫中心所获取的信息。
 
８ 业务委托
 * 厚生劳动省可能将本APP以及邮件服务台和呼叫中心的全部或部分业务委托给第三方（不仅包 括厚生劳动省的直接业务委托，还包括该业务受委托方所进行的二次委托。），并由该受托方负责处理通过本APP或通过邮件服务台和呼叫中心所获取的信息。此种情况下，厚生劳动省会对该受托方进行管理和监督，敦促其采取适当的安全管理措施。
 * 与厚生劳动省有业务委托关系，并且会处理通过本APP或通过邮件服务台和呼叫中心所获取的信息的受托方的详细信息，厚生劳动省将在接触确认APP的主页上进行公开。
 
９ 个人隐私问题的咨询方式
 * 本APP使用过程中涉及到个人隐私的相关问题的信息，将登载于本APP内或厚生劳动省的接触确认APP相关网站内，请向厚生劳动省指定的咨询窗口进行咨询。
