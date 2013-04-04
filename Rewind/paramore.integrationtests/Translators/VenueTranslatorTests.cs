﻿using System;
using System.IO;
using System.Xml;
using Machine.Specifications;
using Paramore.Adapters.Infrastructure.Repositories;
using Paramore.Adapters.Presentation.API.Resources;
using Paramore.Adapters.Presentation.API.Translators;
using Paramore.Domain.Common;
using Paramore.Domain.Venues;
using System.Runtime.Serialization;
using Version = Paramore.Adapters.Infrastructure.Repositories.Version;

namespace paramore.integrationtests.Translators
{
    [Subject("Check that we can get the venue list out of the thin read layer")]
    public class When_changing_a_document_to_a_resource
    {
        private static readonly VenueTranslator venueTranslator = new VenueTranslator();
        private static VenueDocument document;
        private static VenueResource resource;

        Establish context = () =>
            {
                document = new VenueDocument(
                    id: new Id(Guid.NewGuid()),
                    version: new Version(1),
                    venueName: new VenueName("Test Venue"),
                    address: new Address(new Street("MyStreet"), new City("London"), new PostCode("N1 3GT")),
                    venueMap: new VenueMap(new Uri("http://www.mysite.com/maps/12345")),
                    venueContact: new VenueContact(new ContactName("Ian"), new EmailAddress("ian@huddle.com"), new PhoneNumber("123454678")));

            };

        Because of = () => resource = venueTranslator.Translate(document);

        It should_set_the_self_uri = () => resource.Self.ToString().ShouldEqual(string.Format("<link rel='self' href='//{0}/venue/{1}'>", ParamoreGlobals.HostName, document.Id));
        It should_set_the_map_link = () => resource.Map.ToString().ShouldEqual(string.Format("<link rel='map' href='{0}'>", document.VenueMap));
        It should_set_the_version = () => resource.Version.ShouldEqual(document.Version);
        It should_set_the_venue_name = () => resource.Name.ShouldEqual(document.VenueName);
        It should_set_the_address = () => resource.Address.ShouldEqual(document.Address);
        It should_set_the_contact = () => resource.Contact.ShouldEqual(document.VenueContact);

    }

    [Subject("Check that we serialize to the expected xml")]
    public class When_serializing_a_resource_to_xml
    {
        private static DataContractSerializer serializer;
        private static StringWriter stringwriter;
        private static VenueResource resource;
        private static string response;

        Establish context = () =>
            {
                serializer = new DataContractSerializer(typeof(VenueResource));
                stringwriter = new StringWriter();
                resource = new VenueResource(
                    id: Guid.NewGuid(),
                    version: 1,
                    name: "Test Venue",
                    address: "Street : StreetNumber: , Street: MyStreet, City : London, PostCode : N1 3GT",
                    mapURN: "http://www.mysite.com/maps/12345",
                    contact: "ContactName: Ian, EmailAddress: ian@huddle.com, PhoneNumber: 123454678"
                );
            };

        Because of = () =>
            {
                using (var writer = new XmlTextWriter(stringwriter) { Formatting = Formatting.Indented })
                {
                    serializer.WriteObject(writer, resource);
                }
                response =  stringwriter.GetStringBuilder().ToString();
            };

        It should_format_the_self_uri_as_expected = () => { };

    }
}