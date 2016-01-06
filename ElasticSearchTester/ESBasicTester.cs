using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Elasticsearch.Net;
using ElasticSearchTester.Domain;
using ElasticSearchTester.Extensions;
using ElasticSearchTester.Json.Resolvers;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ElasticSearchTester
{
    public class ESBasicTester
    {
        private readonly IIdentity callerCredentials;
        private readonly string user;
        private readonly string localSourcePath;

        public ESBasicTester()
        {
            this.callerCredentials = WindowsIdentity.GetCurrent();

            this.user = this.callerCredentials == null ? "No windows identity" : this.callerCredentials.Name;
            
            var list = new List<string>(Directory.GetCurrentDirectory().Split('\\'));
            list.RemoveAt(list.Count - 1);
            list.RemoveAt(list.Count - 1);
            list.Add("Resources");
            this.localSourcePath = string.Join("\\", list.ToArray());
        }

        [Fact]
        public void TestConnection()
        {
            var client = MakeElasticClient("person-repo");
            Assert.NotNull(client);

            var response = client.Delete<Person>("person-repo");
            Assert.NotNull(response);

            var sa = client.GetIndexSettings(descriptor => descriptor.Local());
            //sa.IndexSettings.Settings.
        }

        [Fact]
        public void PersistenceTest()
        {
            var client = MakeElasticClient("person-repo");

            // cancella il documento con id pari ad 1.
            // nota: viene cancellato il documento con l'id indicato, associato all'indice di default.
            //       altrimenti, se si desidera, si potrebbe indicare l'indice su cui cancellare il documento.

            client.Delete<Person>("1");

            var person = new Person
            {
                //IdSession = "1",
                Firstname = "Martijn",
                Lastname = "Laarman",
                Counter = 2
            };

            // persisto il documento dentro l'indice associato (in questo caso quello di default).
            var indexResponse = client.Index(person,
                descriptor => descriptor.Id("")
                );

            Assert.NotNull(indexResponse);
            Assert.True(indexResponse.Created);
            Assert.Equal(indexResponse.Id, "1");

        }

        [Fact]
        public void BulkPersistenceTest()
        {
            var client = MakeElasticClient("person-new");
            

            // cancellazione massiva dei documenti ...
            var bulkDeleteResponse = client.Bulk(descriptor => descriptor
                .DeleteMany<Person>(new List<string> { "1", "2", "3", "4", "5" })
                .Refresh()
                )
                ;

            // no scope!
            Assert.NotNull(bulkDeleteResponse);

            // inserimento massivo dei documenti specificati.
            var bulkResponse = client.Bulk(descriptor => descriptor
                .IndexMany(
                    new List<Person>
                    {
                        new Person
                        {
                            Id = "1",
                            Firstname = "first_1",
                            Lastname = "last_1",
                            Counter = 11
                        },
                        new Person
                        {
                            Id = "2",
                            Firstname = "first_2",
                            Lastname = "last_2",
                            Counter = 20
                        },
 
                        new Person
                        {
                            Id = "3",
                            Firstname = "first_3",
                            Lastname = "last_3",
                            Counter = 100
                        },
                        new Person
                        {
                            Id = "4",
                            Firstname = "first_4",
                            Lastname = "last_4",
                            Counter = 9
                        },
                        new Person
                        {
                            Id = "5",
                            Firstname = "first_5",
                            Lastname = "last_5",
                            Counter = 9
                        }
 
                    }
                // possiamo specificare un indice diverso da quello di default..
                //, (indexDescriptor, person) => indexDescriptor.Index("person-new")
                )
                );

            Assert.NotNull(bulkResponse);
            Assert.True(bulkResponse.IsValid);
            Assert.False(bulkResponse.Errors);
            Assert.Equal(0, bulkResponse.ItemsWithErrors.Count());

        }

        [Fact]
        public void TestLoadPeople()
        {
            var client = MakeElasticClient("person-search");

            var bulkDeleteResponse = client.Bulk(descriptor => descriptor
                .DeleteMany<Person>(new List<string> { "1", "2", "3", "4", "5" })
                .Refresh()
                )
                ;

            Assert.NotNull(bulkDeleteResponse);

            var bulkResponse = client.Bulk(descriptor => descriptor
                .IndexMany(
                    new List<Person>
                    {
                        new Person
                        {
                            Id = "1",
                            Firstname = "first_1",
                            Lastname = "last_1",
                            Counter = 11
                        },
                        new Person
                        {
                            Id = "2",
                            Firstname = "first_2",
                            Lastname = "last_2",
                            Counter = 20
                        },
 
                        new Person
                        {
                            Id = "3",
                            Firstname = "first_3",
                            Lastname = "last_3",
                            Counter = 100
                        },
                        new Person
                        {
                            Id = "4",
                            Firstname = "first_4",
                            Lastname = "last_4",
                            Counter = 9
                        },
                        new Person
                        {
                            Id = "5",
                            Firstname = "first_5",
                            Lastname = "last_5",
                            Counter = 9
                        }
 
                    }
                )
                .Refresh()
                );

            Assert.NotNull(bulkResponse);
            Assert.False(bulkResponse.Errors);
            Assert.Equal(0, bulkResponse.ItemsWithErrors.Count());

            // ricerco i documenti appena salvati !.
            var searchResponse1 = client.Search<Person>(s => s
                    .From(0)
                    .Size(100)
                    .Query(q =>
                        q.Range(zz => zz
                            .GreaterOrEquals(100)
                            .OnField("counter"))
                    )
                );

            Assert.NotNull(searchResponse1);
            Assert.Equal(1, searchResponse1.Documents.Count());
            Assert.NotNull(searchResponse1.Documents.FirstOrDefault(person => person.Id == "3"));

            // trova i documenti i cui valore [counter] sia tra 10 & 100 (limiti non compresi).
            var searchResponse2 = client.Search<Person>(s => s
                    .From(0)
                    .Size(100)
                    .Query(q =>
                        q.Range(zz => zz.Greater(10)
                            .Lower(100)
                            .OnField(person => person.Counter)
                            )
                    )
                );

            Assert.NotNull(searchResponse2);
            Assert.Equal(2, searchResponse2.Documents.Count());
            Assert.NotNull(searchResponse2.Documents.FirstOrDefault(person => person.Id == "1"));
            Assert.NotNull(searchResponse2.Documents.FirstOrDefault(person => person.Id == "2"));

            // si ricerca un documento con un "termine" in [Firstname] di cui il valore sia uguale al valore "first_1"
            var searchResponse3 = client.Search<Person>(s => s
                    .From(0)
                    .Size(100)
                    .Query(q => q.Term(person => person.Firstname, "first_1"))

                );

            Assert.Equal(searchResponse3.Documents.Count(), 1);
            Assert.NotNull(searchResponse3.Documents.FirstOrDefault(person => person.Id == "1"));
        }


        [Fact]
        public void TestOnAttachment()
        {
            var client = MakeElasticClient("attachment-repo-demo1");

            // cancellando l'indice, cancello anche tutti i documenti associati a questo indice.
            client.DeleteIndex(descriptor => descriptor.Index("attachment-repo-demo1"));

            FileInfo info = new FileInfo(this.localSourcePath + @"\Test1.txt");
            Attachment attachment = new Attachment(info, this.user);
            dynamic dynReference = attachment;

            dynReference.AnotherProp = "dynamic field ...";
            attachment["NumDynamicProp"] = 2;

            // nota: non si può passare un riferimento dinamico alla chiamata di salvataggio del documento.
            // altrimenti accade un loop infinito !!! (almeno credo)..
            var indexResponse = client.Index(attachment);

            Assert.NotNull(indexResponse);
            Assert.True(indexResponse.Created);

            var getResponse = client.Get<Attachment>(indexResponse.Id);

            Assert.NotNull(getResponse);
            Assert.NotNull(getResponse.Source);

            dynamic dynResponse = getResponse.Source;

            Assert.Equal("dynamic field ...", dynResponse.AnotherProp);
            Assert.Equal(2, dynResponse.NumDynamicProp);


            // modifico la struttra dell'oggetto da salvare attraverso il codice dynamico.
            dynReference.MyCustomProp = "custom property!";
            indexResponse = client.Index(attachment);

            Assert.NotNull(indexResponse);
            Assert.True(indexResponse.Created);

            getResponse = client.Get<Attachment>(indexResponse.Id);
            Assert.NotNull(getResponse);
            Assert.NotNull(getResponse.Source);

            dynResponse = getResponse.Source;
            Assert.Equal("dynamic field ...", dynResponse.AnotherProp);
            Assert.Equal(2, dynResponse.NumDynamicProp);
            Assert.Equal("custom property!", dynResponse.MyCustomProp);

        }

        [Fact]
        public void TestVerifyBeforePersistingData()
        {
            FileInfo info = new FileInfo(this.localSourcePath + @"\Test1.txt");
            Attachment att1 = new Attachment(info, this.user);

            var client = MakeElasticClient("attachment-repo-t");

            client.DeleteIndex(new DeleteIndexRequest(new IndexNameMarker().Name = "attachment-repo-t"));
            client.Index(att1);

            // dopo aver persistito l'oggetto chiamante, occorre fare il refresh del client
            // affinché l'oggetto appena salvato possa essere ritrovato in modo immediato !!!
            client.Refresh();

            var response = client.Search<Attachment>(s => s
                .Query(q => q
                    .Term(p => p.Name, "Test1".ToLower())
                    )
                .Query(q => q.MatchPhrase(dd => dd.OnField("field").Query("valore")))
                    

                )
            ;
            //.MatchPhrase(st => st.OnField(attachment => attachment.DataEncoded).Query("Q2lhbyBhIHR1dHRpICEhIQ0KdmVyc2lvbmUgMi4="))            //OK

            Assert.NotNull(response);
            Assert.NotNull(response.Documents);
            Assert.Equal(response.Documents.Count(), 1);

        }


        [Fact]
        //[Description("Questo metodo carica tutti i documenti preseti nella directory di input salvandoli sullo storage di ES, per poi recuperarli e scrivere i file.")]
        public void WrittingDocsFromIndex()
        {
            var client = MakeElasticClient("test-repo");
            client.DeleteIndex(new DeleteIndexRequest(new IndexNameMarker().Name = "test-repo"));

            var files = Directory.GetFiles(this.localSourcePath, "*", SearchOption.TopDirectoryOnly);
            // salvo ogni istanza recuperata dala directory..
            foreach (var file in files)
            {
                var attachment = new Attachment(file, this.user);
                client.Index(attachment);
            }

            // Occorre fare prima il refresh dei date appena salvati sullo store.
            client.Refresh();
            var searchResponse = client.Search<Attachment>(s => s
                .From(0)
                .Size(100)
                );

            // percorso di output.
            string pattern = Path.GetTempPath() + @"\output\{0}_out.{1}";
            foreach (var doc in searchResponse.Documents)
            {
                string fullPath = string.Format(pattern, doc.Name, doc.Extension);
                File.Delete(fullPath);
                using (FileStream file = File.Create(fullPath))
                {
                    byte[] stream = doc.DataEncoded.DecodeFrom64();
                    file.Write(stream, 0, stream.Length);
                }
            }
            Console.WriteLine("Documenti persistiti e riscritti nella cartella di output ...");
        }

        [Fact]
        //[Description("I documenti vengono ritrovati usando direttamente lucene syntax.")]
        public void TestWithLucene()
        {
            var client = MakeElasticClient("test-repo");

            //
            var res0 = client.Search<Attachment>(descriptor => descriptor
                    .QueryString("name: \"TRF2\"")
                );

            Assert.NotNull(res0);
            Assert.Equal(1, res0.Documents.Count());

            //
            var res1 = client.Search<Attachment>(descriptor => descriptor
                    .QueryString("extension: \"pdf\"")
                );

            Assert.NotNull(res1);
            Assert.Equal(2, res1.Documents.Count());


            var res2 = client.Search<Attachment>(descriptor => descriptor
                    .QueryString("dataEncoded: \"Q2lhbyBhIHR1dHRpICEhIQ0KdmVyc2lvbmUgMi4=\"")
                );

            Assert.NotNull(res2);
            Assert.Equal(3, res2.Documents.Count());

        }

        [Fact]
        //[Description("Ricerca i documenti in cui siano associate alla parola indicato nella query string.")]
        public void FullTextSearchOnAny()
        {
            var client = MakeElasticClient("test-repo");

            var res1 = client.Search<Attachment>(descriptor => descriptor
                    .QueryString("txt")
                );
            Assert.NotNull(res1);
            Assert.Equal(4, res1.Documents.Count());

            var res2 = client.Search<Attachment>(descriptor => descriptor
                    .QueryString("pdf")
                );
            Assert.NotNull(res2);
            Assert.Equal(2, res2.Documents.Count());


            var res3 = client.Search<Attachment>(descriptor => descriptor
                    .QueryString("TXT")
                );
            Assert.NotNull(res3);
            Assert.Equal(4, res3.Documents.Count());

            JObject body = new JObject {{"query", new JObject {{"term", new JObject {{"extension", ".txt"}}}}}};

            var res4 = client.Search<Attachment>(descriptor => descriptor
                .Query(queryDescriptor => queryDescriptor.Raw(body.ToString(Formatting.None)))
                    //.QueryRaw((new JObject { { "extension", ".txt" } }).ToString(Formatting.None))
                );
            Assert.NotNull(res4);
            Assert.Equal(4, res4.Documents.Count());
        }

        [Fact]
        public void RawJsonQueries()
        {
            var client = MakeElasticClient("test-repo");

            JObject body = new JObject { { "query", new JObject { { "term", new JObject { { "extension", ".txt" } } } } } };
            var res0 = client.Search<Attachment>(descriptor => descriptor
                //.Query(queryDescriptor => queryDescriptor.Raw(body.ToString(Formatting.None)))
                //.QueryRaw("{query_string:{query:\"extension:.pdf\"}}")  //ok!!
                //.QueryRaw("{term:{extension : \"pdf\"}}")  //ok!!
                //.QueryRaw("{term:{extension : \".pdf\"}}")  //ko!!
                //.QueryRaw("{term:{extension : \".pdf\"}}")  //no results!!
                .QueryRaw("{match:{extension : \".pdf\"}}")  //ok
                );
            Assert.NotNull(res0);
            //Assert.Equal(4, res0.Documents.Count());
        }


        [Fact]
        public void VerifyExistenceInstance()
        {
            var client = MakeElasticClient("test-repo");

            var res1 = client.Search<Attachment>(descriptor => descriptor
                    .From(0)
                    .Size(10)
                    .Query(queryDescriptor => queryDescriptor
                        .Term(attachment => attachment.Name, ("Test1").ToLower())
                    )
                );

            var res2 = client.Search<Attachment>(descriptor => descriptor
                    .From(0)
                    .Size(10)
                    .Query(queryDescriptor => queryDescriptor
                        .Term(attachment => attachment.Extension, "pdf")            //ok
                    )
                );

            var res3 = client.Search<Attachment>(descriptor => descriptor
                    .From(0)
                    .Size(10)
                    .Query(queryDescriptor => queryDescriptor
                        .Term(attachment => attachment.Size, 604)                  //ok
                    )
                );

            var res4 = client.Search<Attachment>(descriptor => descriptor
                    .From(0)
                    .Size(10)
                    .Query(queryDescriptor => queryDescriptor
                        .MatchPhrase(st => st.OnField(attachment => attachment.DataEncoded).Query("Q2lhbyBhIHR1dHRpICEhIQ0KdmVyc2lvbmUgMi4="))            //OK
                    )
                );

            Assert.Equal(res1.Documents.Count(), 1);
            Assert.Equal(res2.Documents.Count(), 2);
            Assert.Equal(res3.Documents.Count(), 1);
            Assert.Equal(res4.Documents.Count(), 3);

        }

        [Fact]
        public void TestOnStudent()
        {
            var client = MakeElasticClient("student-repo");

            // si ricrea la medesima istanza per l'operazione dell'Update.
            var student = new Student
            {
                Id = 2,
                Name = "tesT2",
                Size = 30,
                DataEncoded = "Q2lhbyBhIHR1dHRpICEhIQ0KdmVyc2lvbmUgMi4="
            };

            // 
            client.Delete<Student>(2);
            client.Index(student);
            client.Refresh();

            // ottengo l'istanza da aggiornare...
            var getter = client.Get<Student>(descriptor => descriptor.Id(2));

            // asserts ...
            Assert.True(getter.Found);
            Assert.NotNull(getter);
            Assert.NotNull(getter.Source);

            // faccio l'update...
            var updateResponse = client.Update<Student, object>(
                descriptor => descriptor
                    .Id(2)
                    .Doc(new { Name = "fuckingName" })
                    .Version(Convert.ToInt64(getter.Version))
                    .Refresh()
                );

            Assert.NotNull(updateResponse);
            Assert.True(updateResponse.IsValid);
            Assert.NotEqual(updateResponse.Version, getter.Version);

        }

        [Fact]
        //[Description("Verifichiamo accade se l'istanza senza modifiche provoca l'incremento della versione del medessimo documento.")]
        public void NoUpdatingOnInstanceByHashCode()
        {
            var client = MakeElasticClient("student-repo");
            var getter = client.Get<Student>(descriptor => descriptor.Id(2));

            var newDoc = new Student
            {
                Name = getter.Source.Name,
                DataEncoded = getter.Source.DataEncoded
            };

            // le due istanze dovrebbero essere considerate come uguali.
            Assert.Equal(newDoc, getter.Source);

            if (!newDoc.Equals(getter.Source))
                throw new Exception("Le istanze da confrontare non sono uguali.");

            var updateResponse = client.Update<Student, object>(
                descriptor => descriptor
                    .Id(2)
                    .Doc(new { getter.Source.Name, getter.Source.Size, getter.Source.DataEncoded })
                    .Version(Convert.ToInt64(getter.Version))
                    .Refresh()
                );

            Assert.NotNull(updateResponse);
            Assert.True(updateResponse.IsValid);
            Assert.NotEqual(updateResponse.Version, getter.Version);
        }

        [Fact]
        //[Description("Serve per verificare che le versioni precedenti di un documento non sono più recuperabili dalla chiamata attraverso la GET.")]
        public void TestOnOldVersionInstance()
        {
            var client = MakeElasticClient("student-repo");
            var getter = client.Get<Student>(descriptor => descriptor.Id(2));

            Assert.NotNull(getter);
            Assert.NotNull(getter.Source);

            var getter2 = client.Get<Student>(descriptor => descriptor.Id(2).Version(Convert.ToInt64(getter.Version) - 1));

            Assert.NotNull(getter2);
            Assert.Null(getter2.Source);
        }

        [Fact]
        public void TestOnNewForUpdateInstance()
        {
            var client = MakeElasticClient("student-repo");
            Student instance = new Student
            {
                Id = 10,
                Name = "Name10",
                DataEncoded = "DataEncoded10",
                Size = 10
            };
            
            Student instance2 = new Student
            {
                Id = 5,
                Name = "Name5",
                DataEncoded = "DataEncoded5",
                Size = 5
            };
            var resp1 = client.Index(instance);
            Assert.True(resp1.Created);

            var resp2 = client.Index(instance2);
            Assert.True(resp2.Created);

            var updateResponse = client.Update<Student>(descriptor => descriptor.Doc(instance2).Id(10));
            Assert.NotNull(updateResponse);

            client.Delete(instance);
            client.Delete(instance2);

        }

        [Fact]
        //[Description("Inserimento & cancellazione massiva di documenti persistenti.")]
        public void TestWithBulkOperationOnInsert()
        {
            var client = MakeElasticClient("student-repo-bulk");

            var resBulkDelete = client.Bulk(descriptor => descriptor
                        .DeleteMany<Student>(new List<long> { 1, 2, 3, 4, 5 })
                        .Refresh()
                );

            Assert.NotNull(resBulkDelete);
            Assert.False(resBulkDelete.Errors);
            Assert.Equal(resBulkDelete.ItemsWithErrors.Count(), 0);

            var resBulkInsert = client.Bulk(descriptor => descriptor
                    .Create<Student>(z => z.Document(new Student { Id = 1, Name = "name1", Size = 1, DataEncoded = "aa1" }))
                    .Create<Student>(z => z.Document(new Student { Id = 2, Name = "name2", Size = 2, DataEncoded = "aa2" }))
                    .Create<Student>(z => z.Document(new Student { Id = 3, Name = "name3", Size = 3, DataEncoded = "aa3" }))
                    .Create<Student>(z => z.Document(new Student { Id = 4, Name = "name4", Size = 4, DataEncoded = "aa4" }))
                    .Create<Student>(z => z.Document(new Student { Id = 5, Name = "name5", Size = 5, DataEncoded = "aa5" }))

                );

            Assert.NotNull(resBulkInsert);
            Assert.False(resBulkInsert.Errors);
            Assert.Equal(resBulkInsert.ItemsWithErrors.Count(), 0);
        }

        /// <inheritdoc/>
        [Fact]
        //[Description("Inserimento & cancellazione massiva di documenti persistenti.")]
        public void TestWithBulkOperationOnInsert2()
        {
            var client = MakeElasticClient("student-repo-bulk-2");

            client.DeleteByQuery<Student>(descriptor => descriptor.MatchAll());
            client.DeleteIndex(descriptor => descriptor.Index("student-repo-bulk-3"));
            client.Refresh();


            var resBulkInsert = client.Bulk(descriptor => descriptor
                    .Create<Student>(z => z.Document(new Student { Id = 1, Name = "name1", Size = 1, DataEncoded = "aa1" }))
                    .Create<Student>(z => z.Document(new Student { Id = 2, Name = "name2", Size = 2, DataEncoded = "aa2" }))
                    .Create<Student>(z => z.Document(new Student { Id = 3, Name = "name3", Size = 3, DataEncoded = "aa3" }))
                    .Create<Student>(z => z.Document(new Student { Id = 4, Name = "name4", Size = 4, DataEncoded = "aa4" }))
                    .Create<Student>(z => z.Document(new Student { Id = 5, Name = "name5", Size = 5, DataEncoded = "aa5" }))

                    .Create<Student>(z => z.Document(new Student { Id = 6, Name = "name1", Size = 1, DataEncoded = "aa1" }))
                    .Create<Student>(z => z.Document(new Student { Id = 7, Name = "name2", Size = 2, DataEncoded = "aa2" }))
                    .Create<Student>(z => z.Document(new Student { Id = 8, Name = "name3", Size = 3, DataEncoded = "aa3" }))
                    .Create<Student>(z => z.Document(new Student { Id = 9, Name = "name4", Size = 4, DataEncoded = "aa4" }))
                    .Create<Student>(z => z.Document(new Student { Id = 10, Name = "name5", Size = 5, DataEncoded = "aa5" }))

                    .Create<Student>(z => z.Document(new Student { Id = 11, Name = "name1", Size = 2, DataEncoded = "aa1" }))
                    .Create<Student>(z => z.Document(new Student { Id = 12, Name = "name2", Size = 2, DataEncoded = "aa1" }))
                    .Create<Student>(z => z.Document(new Student { Id = 13, Name = "name3", Size = 3, DataEncoded = "aa1" }))
                    .Create<Student>(z => z.Document(new Student { Id = 14, Name = "name4", Size = 4, DataEncoded = "aa2" }))
                    .Create<Student>(z => z.Document(new Student { Id = 15, Name = "name5", Size = 1, DataEncoded = "aa3" }))
                );

            client.Refresh();

            Assert.NotNull(resBulkInsert);
            Assert.False(resBulkInsert.Errors);
            Assert.Equal(resBulkInsert.ItemsWithErrors.Count(), 0);

        }

        [Fact]
        public void MultiGetDocuments()
        {
            var client = MakeElasticClient("student-repo-bulk");

            var multiGet = client.GetMany<Student>(new List<long> { 1, 2, 3, 4, 5 }).ToArray();
            Assert.NotNull(multiGet);

            Assert.Equal(5, multiGet.Length);

            Assert.NotNull(multiGet.All(hit => hit.Found));
            Assert.True(multiGet.All(hit => hit.Source != null));
            Assert.True(multiGet.All(hit => hit.Version != string.Empty));
        }

        [Fact]
        public void Getter1()
        {
            var client = MakeElasticClient("student-repo");
            var getResponse = client.Get<Student>(1);

            Assert.True(getResponse.Found);

            var instance = getResponse.Source;
            Assert.NotNull(instance);
        }

        [Fact]
        public void Getter2()
        {
            var client = MakeElasticClient("student-repo");
            var getResponse = client.Get<Student>
                (
                    descriptor => descriptor.Id(1)
                                    .Fields("id", "name", "size")

                );

            Assert.True(getResponse.Found);

            // e' null forse perché è stato fatto una proiezione dei dati..
            Assert.Null(getResponse.Source);

            // recupero i valori recuperati dalla proiezione della chiamata al servizio..
            var name = getResponse.Fields.FieldValues(p => p.Name);
            var size = getResponse.Fields.FieldValues(p => p.Size);

            Assert.True(name != null && name.Length == 1);
            Assert.True(size != null && size.Length == 1);
        }

        [Fact]
        public void TestScriptField()
        {
            var client = MakeDefaultClient("student-repo");

            var searchRis = client.Search<Student>(qq => qq
                .From(0)
                .Size(100) //.MatchAll()
                .Fields("id", "name", "size")
                .ScriptFields(sq => sq
                    .Add("doubleSize", descriptor => descriptor
                        .Script("doc['size'].value * multiplier")
                        .Params(sp => sp
                            .Add("multiplier", 4)))));

            Assert.True(searchRis.IsValid);
            Assert.Equal(searchRis.Documents.Count(), 0);
            //Assert.Greater(searchRis.Hits.Count(), 0);
            Assert.True(searchRis.Hits.Any());
        }

        [Fact]
        public void TestScriptField2()
        {
            const string dynamicProperty = "$idsession";

            var client = MakeElasticClient("student-repo");
            var resp = client.DeleteByQuery<Student>(descriptor => descriptor.AllTypes().MatchAll());
            Assert.True(resp.IsValid);

            client.DeleteMapping<Student>();
            client.Refresh();

            client.Map<Student>(descriptor => descriptor);

            var mapping = client.GetMapping<Student>();
            if (mapping.IsValid)
            {
                var map = mapping.Mapping;
                //if (!map.Properties.ContainsKey("transaction"))
                //{
                //    client.Map<Student>(descriptor => descriptor
                //        .Properties(propertiesDescriptor => propertiesDescriptor
                //            .NestedObject<Isolation>(mappingDescriptor => mappingDescriptor.Name("transaction"))));
                //}
            }

            var student = new Student
            {
                Id = 11,
                DataEncoded = null,
                Name = "NOME1",
                Size = 14
            };

            dynamic instance = student.AsDynamic(
                new KeyValuePair<string, object>(dynamicProperty, "100"));

            object r = instance;

            string id = client.Infer.Id(student);
            
            // It doesn't work
            //var response0 = client.Index(r, descriptor => descriptor.Type<Student>().IdFrom(student));

            // ok
            var response1 = client.Index(r, descriptor => descriptor
                .Type<Student>()
                .Id(id));

            Assert.True(response1.IsValid);
            Assert.Equal(student.Id.ToString(), response1.Id);

            student = new Student
            {
                Id = 22,
                DataEncoded = "sdnjkvndskjVJKSN",
                Name = "NOME1",
                Size = 14
            };

            var response2 = client.Index(student);

            Assert.True(response2.IsValid);
            Assert.Equal(student.Id.ToString(), response2.Id);

            student = new Student
            {
                Id = 33,
                DataEncoded = "sdnjkvndskjVJKSN",
                Name = "NOME1",
                Size = 14
            };

            instance = student.AsDynamic(
                new KeyValuePair<string, object>(dynamicProperty, "200"));
            
            id = client.Infer.Id(student);
            r = instance;

            var response3 = client.Index(r, descriptor => descriptor
                .Type<Student>()
                .Id(id));

            Assert.True(response3.IsValid);
            Assert.Equal(student.Id.ToString(), response3.Id);
        }


        /// <summary>
        /// Test on searching using advamced filtering....
        /// </summary>
        [Fact]
        public void TestOnSearching()
        {
            const string dynamicProperty = "$idsession";
            var client = MakeDefaultClient("student-repo");
            
            var searchResponse = client.Search<Student>(descriptor => descriptor
                .From(0)
                .Take(10)
                .Version()
                .Filter(fd => fd
                    .Or(fd1 => fd1.Missing(dynamicProperty),
                        fd2 => fd2.And(
                            fd22 => fd22.Exists(dynamicProperty),
                            fd23 => fd23.Term(dynamicProperty, "500")
                        )
                    )
                )
            );

            // questo codice funziona....
            // quindi dynamics si puo' utilizzare...
            var res = client.Get<dynamic>(descriptor => descriptor.Id(33).Type<Student>().Index("student-repo"));
            Assert.NotNull(res);
            Assert.NotNull(res.Source);

            var res0 = client.Update<Student>(descriptor => descriptor
                .Id(22)
                //.Type<Student>()
                //.Script("ctx._source._idsession = idsession")
                //.Params(p => p
                //        .Add("idsession", (string)null))
                .Script("ctx._source.remove(\"\\$idsession\")")
                );


            //var res2 = client.Update<dynamic>(descriptor => descriptor
            //    .Id(33)
            //    .Type<Student>()
            //    .Script("ctx._source.remove(\"_idsession\")")
            //    );

            Assert.NotNull(res0);
            Assert.True(res0.IsValid);



            Assert.True(searchResponse.IsValid);
            Console.WriteLine(searchResponse.Hits.Count());

            var searcher = new SearchRequest("student-repo", client.Infer.TypeName<Student>())
            {
                From = 0,
                Size = 10,
                Version = true,
                Filter = new FilterDescriptor<Student>()
                    .Or(fd1 => fd1.Missing("_idsession"),
                        fd2 => fd2.And(
                            fd22 => fd22.Exists("_idsession"),
                            fd23 => fd23.Term("_idsession", "500")
                            )
                    )
            };

            searchResponse = client.Search<Student>(searcher);
            Assert.True(searchResponse.IsValid);
            Console.WriteLine(searchResponse.Hits.Count());
        }

        [Fact]
        public void TestScriptFields()
        {
            // I make a client instance with the given default index... 
            ElasticClient client = MakeDefaultClient("student-test");

            var bulkDelete = client.Bulk(descriptor => descriptor
                    .Delete<Student>(qq => qq.Id(1))
                    .Delete<Student>(qq => qq.Id(2))
                );

            Assert.NotNull(bulkDelete);
            Assert.True(bulkDelete.IsValid);

            var res1 = client.Index(
                    new Student
                    {
                        Id = 1,
                        Name = "Naming1",
                        Size = 100
                    }
                );

            Assert.NotNull(res1);
            Assert.True(res1.IsValid);

            var res2 = client.Index(
                    new Student
                    {
                        Id = 2,
                        Name = "Naming1",
                        Size = 100
                    }
                );
            Assert.NotNull(res2);
            Assert.True(res2.IsValid);

            // making a refresh...
            client.Refresh();

            var searchGoodRes = client.Search<Student>(qq => qq
                .From(0)
                .Size(10)
                );

            Assert.NotNull(searchGoodRes);
            Assert.True(searchGoodRes.IsValid);
            Assert.Equal(2, searchGoodRes.Documents.Count());

            var searchWrongRis = client.Search<Student>(qq => qq
                .From(0)
                .Size(10)
                .MatchAll()
                .Fields("id", "name", "size")
                .ScriptFields(sq => sq
                    .Add("doubleSize", descriptor => descriptor
                        .Script("doc['size'].value * multiplier")
                        .Params(sp => sp
                            .Add("multiplier", 4)
                        )
                    )
                )
                );

            // this is the last change present in the last version
            // It seems Once the output instance were counted ..
            Assert.Equal(searchWrongRis.Documents.Count(), 0);

            // the hints number must be equals to total number of instances.. so 2.
            Assert.Equal(searchWrongRis.Hits.Count(), 2);

            // why is this invalid???
            Assert.True(searchWrongRis.IsValid);
        }

        [Fact]
        public void AggregationSearcher1()
        {
            var client = MakeDefaultClient("student-repo");
            var res = client.Search<Student>(qq => qq
                    .Aggregations(a => a
                        .Terms("my_agg", t => t
                            .Field(student => student.Name)
                        )
                    )
                );

            Assert.NotNull(res);
            Assert.True(res.IsValid);

            var agg = res.Aggs.Terms("my_agg");
            Assert.NotNull(agg);
            Assert.Equal(1, agg.Items.Count);
        }

        [Fact]
        public void AggregationSearcher2()
        {
            var client = MakeElasticClient("test-repo");
            var result = client.Search<Attachment>(qq => qq
                //.Query(descriptor => descriptor.MatchPhrase(queryDescriptor => queryDescriptor.OnField(attachment => attachment.Extension).Query(string.Empty)))
                    .Aggregations(a => a
                        .Terms("my_agg", t => t
                            .Field(attachment => attachment.Extension)
                        )
                        .Terms("my_count", t => t
                            .Field(att => att.Name)
                        )
                    )

                );

            Assert.NotNull(result);
            Assert.True(result.IsValid);

            var agg = result.Aggs.Terms("my_agg");
            Assert.NotNull(agg);

            var agg1 = result.Aggs.Terms("my_count");
            Assert.NotNull(agg1);

            Assert.Equal(2, agg.Items.Count);
        }

        [Fact]
        public void AggregationSearcher3()
        {
            var client = MakeElasticClient("test-repo");
            var result = client.Search<Attachment>(qq => qq
                    .Aggregations(a => a
                        .ValueCount("my_count", t => t
                            .Field(att => att.Name)
                        )
                    )

                );

            Assert.NotNull(result);
            Assert.True(result.IsValid);

            var agg = result.Aggs.ValueCount("my_count");
            Assert.NotNull(agg);
            Assert.Equal(7, agg.Value);
        }

        [Fact]
        public void AggregationSearcher4()
        {
            var client = MakeElasticClient("student-repo-bulk-2");
            client.Refresh();

            var result = client.Search<Student>(qq => qq
                    .Aggregations(a => a        
                        .ValueCount("mc2", t => t
                            .Field(att => att.Name)
                        )
                        .ValueCount("mc3", t => t
                            .Field(att => att.Version)
                        )
                        .ValueCount("mc4", t => t
                            .Field(att => att.Size)
                        )
                        .ValueCount("mc5", t => t
                            .Field(att => att.DataEncoded)
                        )
                    )
                );

            var agg2 = result.Aggs.ValueCount("mc2");
            var agg3 = result.Aggs.ValueCount("mc3");
            var agg4 = result.Aggs.ValueCount("mc4");
            var agg5 = result.Aggs.ValueCount("mc5");

            Assert.NotNull(agg2);
            Assert.NotNull(agg3);
            Assert.NotNull(agg4);
            Assert.NotNull(agg5);
        }

        [Fact]
        public void AggregationSearcher5_()
        {
            var client = MakeElasticClient("student-repo");
            var result = client.Search<Student>(qq => qq
                    .Aggregations(a => a
                        .Max("id_max", descriptor => descriptor.Field(student => student.Id))
                        .Max("size_max", descriptor => descriptor.Field(student => student.Size))
                    )
                );

            Assert.NotNull(result);
            Assert.True(result.IsValid);

            var aaa = result.Aggs.Max("id_max");
            Assert.NotNull(aaa);
            Assert.Equal(33.0, aaa.Value);

            var bbb = result.Aggs.Max("size_max");
            Assert.NotNull(bbb);
            Assert.Equal(14.0, bbb.Value);


            var result0 = client.Search<Student>(descriptor => descriptor
                .Aggregations(a => a
                    //.Max("id_max2", aggDescriptor => aggDescriptor.Field("_id"))
                    //.Max("id_max2", aggDescriptor => aggDescriptor.Script("ctx._id as long"))
                    //.Max("id_max2", aggDescriptor => aggDescriptor.Script("doc['ciao'].value as long"))
                    .Max("id_max2", aggDescriptor => aggDescriptor.Script("org.elasticsearch.index.mapper.Uid.idFromUid(doc[\"_uid\"].value).toLong()"))
                )
                );

            //Assert.False(result0.IsValid);
            var ccc = result0.Aggs.Max("id_max2");
            Assert.NotNull(ccc);

            var result1 = client.Search<Student>(descriptor => descriptor
                .ScriptFields(sq => sq
                    //.Add("MyProperty", filterDescriptor => filterDescriptor.Script("ctx._id + 1000"))
                    .Add("MyProperty", filterDescriptor => filterDescriptor.Script("doc['id'].value + 1000"))
                    //.Add("MyProperty", filterDescriptor => filterDescriptor.Script("org.elasticsearch.index.mapper.Uid.idFromUid(doc['_uid'].value) + 1000"))
                )
                );
            Assert.NotNull(result1);
            
        }

        [Fact]
        public void AggregationSearchStats()
        {
            var client = MakeElasticClient("test-repo");
            var result = client.Search<Attachment>(qq => qq
                    .From(0)
                    .Size(10)
                    .MatchAll()
                    .Aggregations(a => a
                        .Stats("mystats1", t => t
                            .Field(att => att.Extension)
                        )
                    )
                );

            Assert.NotNull(result.ConnectionStatus.ResponseRaw);

            Assert.NotNull(result);
            Assert.True(result.IsValid);

            var agg1 = result.Aggs.Stats("mystats1");
            Assert.True(agg1.Count == 2);
        }

        [Fact]
        public void AggregationSearcher5()
        {
            var client = MakeElasticClient("student-repo-bulk-2");
            var result = client.Search<Student>(qq => qq
                    .From(0)
                    .Size(100)
                    .Aggregations(a => a
                        .Terms("groupByName", t => t
                            .Field(student => student.Name)
                        )
                        .Terms("groupBySize", t => t
                            .Field(student => student.Size)
                        )
                        .Terms("groupByData", t => t
                            .Field(student => student.DataEncoded)
                        )
                        .Terms("groupByDataEncodedSize", t => t
                            .Script("doc['dataEncoded'].value + '-' + doc['size'].value")
                        )
                    )
                );

            Assert.NotNull(result);
            Assert.True(result.IsValid);

            var agg = result.Aggs.Terms("groupByName");
            Assert.NotNull(agg);
            Assert.Equal(5, agg.Items.Count);
            Assert.True(agg.Items.All(item => item.DocCount == 3));

            var agg1 = result.Aggs.Terms("groupBySize");
            Assert.NotNull(agg1);
            Assert.Equal(5, agg1.Items.Count);

            var agg2 = result.Aggs.Terms("groupByData");
            Assert.NotNull(agg2);
            Assert.Equal(5, agg2.Items.Count);

            var agg3 = result.Aggs.Terms("groupByDataEncodedSize");
            Assert.NotNull(agg3);
        }

        [Fact]
        public void AggregateSearch1()
        {
            var client = MakeElasticClient("companies");

            client.Bulk(descriptor => descriptor.DeleteMany<Company>(
                new List<long> { 1, 2, 3, 4, 5 },
                (deleteDescriptor, s) => deleteDescriptor.Index("companies")
                )
                );

            client.Index(
                new Company
                {
                    Id = 1,
                    Name = "SBI",
                    Employers = new List<Employer>
                    {
                        new Employer { Name = "Danie1", Surname = "Danie1_Sur", Salary = 12000},
                        new Employer { Name = "Jon1", Surname = "Jon1_Sur", Salary = 15050},
                        new Employer { Name = "Max1", Surname = "Max1_Sur", Salary = 25000},
                        new Employer { Name = "Emma1", Surname = "Emma1_Sur", Salary = 35500}
                    }

                }
                );
            client.Index(
                new Company
                {
                    Id = 2,
                    Name = "SBI-2",
                    Employers = new List<Employer>
                    {
                        new Employer { Name = "Danie2", Surname = "Danie2_Sur", Salary = 12000},
                        new Employer { Name = "Jon2", Surname = "Jon2_Sur", Salary = 15050},
                        new Employer { Name = "Max2", Surname = "Max2_Sur", Salary = 25000},
                        new Employer { Name = "Emma2", Surname = "Emma2_Sur", Salary = 35500}
                    }

                }
                );
            client.Index(
                new Company
                {
                    Id = 3,
                    Name = "SBI-3",
                    Employers = new List<Employer>
                    {
                        new Employer { Name = "Danie3", Surname = "Danie3_Sur", Salary = 12000},
                        new Employer { Name = "Jon3", Surname = "Jon3_Sur", Salary = 15050},
                        new Employer { Name = "Max3", Surname = "Max3_Sur", Salary = 25000},
                        new Employer { Name = "Emma3", Surname = "Emma3_Sur", Salary = 35500}
                    }

                }
                );
            client.Index(
                new Company
                {
                    Id = 4,
                    Name = "SBI-4",
                    Employers = new List<Employer>
                    {
                        new Employer { Name = "Danie3", Surname = "Danie4_Sur", Salary = 12000},
                        new Employer { Name = "Jon3", Surname = "Jon4_Sur", Salary = 15050},
                        new Employer { Name = "Max3", Surname = "Max4_Sur", Salary = 25000},
                        new Employer { Name = "Emma3", Surname = "Emma4_Sur", Salary = 35500}
                    }

                }
                );

            client.Refresh();
        }

        [Fact]
        public void Test1()
        {
            //doc['size'].value

            var client = MakeElasticClient("companies");
            var result = client.Search<Company>(q => q
                .Aggregations(a => a
                    .Cardinality("card", c => c
                        .Field(company => company.Employers.First().Name)
                    //.Script("doc['employers'][0].name")   //gli script non funzionano.
                    )

                )

                );

            var res = result.Aggs.Cardinality("card");
            Assert.NotNull(res);
            Assert.NotNull(result);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void TestOnSinger()
        {
            const string index = "singers";
            var client = MakeElasticClient(index);
            client.DeleteMapping<Singer>();

            var res = client.Map<Singer>(descriptor => descriptor.Index(index)
                .IdField(mappingDescriptor => mappingDescriptor.Path("key"))
                );
            Assert.NotNull(res);

            var mapping = client.GetMapping<Singer>(descriptor => descriptor.Index(index));
            Assert.NotNull(mapping);

            var map = mapping.Mapping;
            Assert.NotNull(map);
            Assert.NotNull(map.IdFieldMappingDescriptor);

            var instance = new Singer
            {
                Key = 5,
                Name = "Nome di test"
            };

            var response = client.Index(instance);
            Assert.True(response.IsValid);
            Assert.Equal(5.ToString(CultureInfo.InvariantCulture), response.Id);

            var id = client.Infer.Id(new Singer
            {
                Key = 10,
                Name = "myname"
            });

            Assert.Null(id);
        }

        [Fact]
        public void GetMappingAtt()
        {
            var client = MakeElasticClient("test-repo");

            var mapping = client.GetMapping<Attachment>(descriptor => descriptor
                .Index("test-repo")
                );
            Assert.NotNull(mapping);
            Assert.True(mapping.IsValid);
            Assert.NotNull(mapping.Mapping.Properties);

            client.CreateIndex("", descriptor => descriptor.AddMapping<Student>(p => p.Dynamic())
                );

            Assert.NotNull(mapping);
            Assert.True(mapping.IsValid);
            Assert.NotNull(mapping.Mapping.Properties);

        }


        private static ElasticClient MakeElasticClient(string defaultIndex)
        {
            var list = new List<Type>
            {
                //typeof(SearchDescriptor<>), typeof(DeleteByQueryDescriptor<>), typeof(QueryPathDescriptorBase<,,>)
                typeof(QueryPathDescriptorBase<,,>)
            };

            var settings = MakeSettings(defaultIndex)
                .ExposeRawResponse();

            settings.SetJsonSerializerSettingsModifier(
                delegate(JsonSerializerSettings zz)
                {
                    //zz.NullValueHandling = NullValueHandling.Include;
                    zz.MissingMemberHandling = MissingMemberHandling.Ignore;
                    zz.TypeNameHandling = TypeNameHandling.Auto;
                    zz.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
                    zz.ContractResolver = new DynamicContractResolver(settings);
                }
                );
            
            return new ElasticClient(settings, null, new MoreThanNestSerializer(settings, list));
        }


        private static ElasticClient MakeDefaultClient(string defaultIndex)
        {
            var settings = MakeSettings(defaultIndex);
            return new ElasticClient(settings);
        }


        private static ConnectionSettings MakeSettings(string defaultIndex)
        {
            var uri = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(uri, defaultIndex);
            return settings;
        }

    }
}
