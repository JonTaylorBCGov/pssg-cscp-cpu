﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Cscp.VictimServices.Public.JsonObjects;
using Gov.Cscp.VictimServices.Public.ViewModels;
using Gov.Cscp.VictimServices.Public.Infrastructure;

namespace Gov.Cscp.VictimServices.Public.Models.Extensions
{
    public static class ApplicationModelExtensions
    {
        public static ApplicationDynamicsModel ToVsdVictimsModel(this ApplicationFormModel model)
        {
            var application = GetApplicationDefaults();
            application.Application.vsd_applicanttype = (int)ApplicationType.Victim;

            if (model == null)
                return null;

            if (model.PersonalInformation != null)
            {
                application.Application.vsd_applicantsfirstname = model.PersonalInformation.firstName;
                application.Application.vsd_applicantsmiddlename = model.PersonalInformation.middleName;
                application.Application.vsd_applicantslastname = model.PersonalInformation.lastName;
                application.Application.vsd_otherfirstname = model.PersonalInformation.otherFirstName;
                application.Application.vsd_otherlastname = model.PersonalInformation.otherLastName;
                if (model.PersonalInformation.dateOfNameChange.HasValue)
                {
                    application.Application.vsd_dateofnamechange = model.PersonalInformation.dateOfNameChange;
                }

                application.Application.vsd_applicantsgendercode = model.PersonalInformation.gender;
                if (model.PersonalInformation.birthDate.HasValue)
                {
                    application.Application.vsd_applicantsbirthdate = model.PersonalInformation.birthDate.Value;
                }
                application.Application.vsd_applicantsmaritalstatus = model.PersonalInformation.maritalStatus;

                application.Application.vsd_applicantsoccupation = model.PersonalInformation.occupation;
                application.Application.vsd_applicantssocialinsurancenumber = model.PersonalInformation.sin;

                // what format does dynamics expect in the JSON?
                // currently the Dynamics UI only allows a 10-digit number and uses some fancy input masking to include the parens and hyphens 
                // Form side should mimic the fancy javascript input masking from the Dynamics UI, and probably just represent it as a pure 10 digit number behind the scenes and in the JSON
                // Ideally the whole thing would support international numbers and use the E.164 standard for behind-the-scenes representation
                // (see https://www.sitepoint.com/working-phone-numbers-javascript/ for inspiration)
                // but for now we should only support whatever the Dynamics UI supports - no sense adding extra features that can't be used because of the Dynamics side
                application.Application.vsd_applicantsprimaryphonenumber = model.PersonalInformation.phoneNumber;
                application.Application.vsd_applicantsalternatephonenumber = model.PersonalInformation.alternatePhoneNumber;
                application.Application.vsd_applicantsemail = model.PersonalInformation.email;
                application.Application.vsd_applicantspreferredmethodofcontact = model.PersonalInformation.preferredMethodOfContact;

                if (model.PersonalInformation.primaryAddress != null)
                {
                    application.Application.vsd_applicantsprimaryaddressline1 = model.PersonalInformation.primaryAddress.line1;
                    application.Application.vsd_applicantsprimaryaddressline2 = model.PersonalInformation.primaryAddress.line2;
                    application.Application.vsd_applicantsprimarycity = model.PersonalInformation.primaryAddress.city;
                    application.Application.vsd_applicantsprimaryprovince = model.PersonalInformation.primaryAddress.province;
                    application.Application.vsd_applicantsprimarycountry = model.PersonalInformation.primaryAddress.country;
                    application.Application.vsd_applicantsprimarypostalcode = model.PersonalInformation.primaryAddress.postalCode;
                }

                if (model.PersonalInformation.alternateAddress != null)
                {
                    application.Application.vsd_applicantsalternateaddressline1 = model.PersonalInformation.alternateAddress.line1;
                    application.Application.vsd_applicantsalternateaddressline2 = model.PersonalInformation.alternateAddress.line2;
                    application.Application.vsd_applicantsalternatecity = model.PersonalInformation.alternateAddress.city;
                    application.Application.vsd_applicantsalternateprovince = model.PersonalInformation.alternateAddress.province;
                    application.Application.vsd_applicantsalternatecountry = model.PersonalInformation.alternateAddress.country;
                    application.Application.vsd_applicantsalternatepostalcode = model.PersonalInformation.alternateAddress.postalCode;
                }
            }

            if (model.CrimeInformation != null)
            {
                application.Application.vsd_cvap_typeofcrime = model.CrimeInformation.typeOfCrime;
                if (model.CrimeInformation.crimePeriodStart.HasValue)
                {
                    application.Application.vsd_cvap_crimestartdate = model.CrimeInformation.crimePeriodStart;
                }
                if (model.CrimeInformation.crimePeriodEnd.HasValue)
                {
                    application.Application.vsd_cvap_crimeenddate = model.CrimeInformation.crimePeriodEnd;
                }
                application.Application.vsd_cvap_reasontoapplylate = model.CrimeInformation.whyDidYouNotApplySooner; // TODO: Verify mapping - I think it's right but just different names
                application.Application.vsd_cvap_crimelocations = model.CrimeInformation.crimeLocation;
                application.Application.vsd_cvap_crimedetails = model.CrimeInformation.crimeDetails;
                application.Application.vsd_cvap_injuries = model.CrimeInformation.crimeInjuries;

                // TODO: Fix file mapping
                // how does the uploading work? is our array a list of file ids? Look them up and extract appropriate data...
                //application.DocumentCollection = model.CrimeInformation.additionalInformationFiles.Select(f => new Documentcollection
                //{
                //    body = "ALLTHEBYTES",
                //    filename = "tempfile.txt"
                //}).ToArray();

                application.Application.vsd_cvap_reportedtopolice = model.CrimeInformation.wasReportMadeToPolice;
                application.Application.vsd_cvap_policedetachment = model.CrimeInformation.policeReportedWhichPoliceForce;
                if (model.CrimeInformation.policeReportedDate.HasValue)
                {
                    application.Application.vsd_cvap_policereportingstartdate = model.CrimeInformation.policeReportedDate;
                }
                if (model.CrimeInformation.policeReportedEndDate.HasValue)
                {
                    application.Application.vsd_cvap_policereportingenddate = model.CrimeInformation.policeReportedEndDate;
                }
                application.Application.vsd_cvap_crimereportedto = model.CrimeInformation.noPoliceReportIdentification; // TODO: verify mapping - I think it's right, but different names

                application.PoliceFileNumberCollection = model.CrimeInformation.policeReports.Select(r => new Policefilenumbercollection
                {
                    vsd_investigatingpoliceofficername = r.investigatingOfficer,
                    vsd_policefilenumber = r.policeFileNumber
                }).ToArray();

                application.Application.vsd_cvap_offenderfirstname = model.CrimeInformation.offenderFirstName;
                application.Application.vsd_cvap_offendermiddlename = model.CrimeInformation.offenderMiddleName;
                application.Application.vsd_cvap_offenderlastname = model.CrimeInformation.offenderLastName;

                application.Application.vsd_cvap_relationshiptooffender = model.CrimeInformation.offenderRelationship;
                application.Application.vsd_cvap_isoffendercharged = model.CrimeInformation.offenderBeenCharged;

                application.CourtInfoCollection = model.CrimeInformation.courtFiles.Select(f => new Courtinfocollection
                {
                    vsd_courtfilenumber = f.courtFileNumber,
                    vsd_courtlocation = f.courtLocation
                }).ToArray();

                application.Application.vsd_cvap_isoffendersued = model.CrimeInformation.haveYouSuedOffender;
                application.Application.vsd_cvap_intentiontosueoffender = model.CrimeInformation.intendToSueOffender;

                if (model.CrimeInformation.racafInformation != null)
                {
                    application.Application.vsd_racaf_appliedforrestitution = model.CrimeInformation.racafInformation.applyToCourtForMoneyFromOffender; 
                    application.Application.vsd_racaf_requestedexpenses = model.CrimeInformation.racafInformation.expensesRequested;
                    application.Application.vsd_racaf_expensesawarded = model.CrimeInformation.racafInformation.expensesAwarded; 
                    application.Application.vsd_racaf_amountreceived = model.CrimeInformation.racafInformation.expensesReceived; 

                    application.Application.vsd_racaf_legalactiontaken = model.CrimeInformation.racafInformation.willBeTakingLegalAction;
                    application.Application.vsd_racaf_lawyerorfirmname = model.CrimeInformation.racafInformation.lawyerOrFirmName;
                    application.Application.vsd_racaf_lawyeraddressline1 = model.CrimeInformation.racafInformation.lawyerAddress?.line1;
                    application.Application.vsd_racaf_lawyeraddressline2 = model.CrimeInformation.racafInformation.lawyerAddress?.line2;
                    application.Application.vsd_racaf_lawyercity = model.CrimeInformation.racafInformation.lawyerAddress?.city;
                    application.Application.vsd_racaf_lawyerprovince = model.CrimeInformation.racafInformation.lawyerAddress?.province;
                    application.Application.vsd_racaf_lawyerpostalcode = model.CrimeInformation.racafInformation.lawyerAddress?.postalCode;
                    application.Application.vsd_racaf_lawyercountry = model.CrimeInformation.racafInformation.lawyerAddress?.country;

                    application.Application.vsd_racaf_signature = model.CrimeInformation.racafInformation.signature;
                }
            }

            if (model.MedicalInformation != null)
            {
                application.Application.vsd_applicantspersonalhealthnumber = model.MedicalInformation.personalHealthNumber;
                application.Application.vsd_applicantsextendedhealthprovidername = model.MedicalInformation.otherHealthCoverageProviderName; // TODO: verify mapping, "other" seems weird here
                application.Application.vsd_applicantsextendedhealthnumber = model.MedicalInformation.otherHealthCoverageExtendedPlanNumber; // TODO: verify mapping, "other" seems weird here

                application.Application.vsd_cvap_treatmenthospitalname = model.MedicalInformation.treatedAtHospitalName;
                if (model.MedicalInformation.treatedAtHospitalDate.HasValue)
                {
                    application.Application.vsd_cvap_treatmentdate = model.MedicalInformation.treatedAtHospitalDate;
                }

                //application.ProviderCollection = model.MedicalInformation.otherTreatments.Select(t => new Providercollection
                //{
                //    vsd_name = t.providerName,
                //    vsd_phonenumber = t.providerPhoneNumber,

                //    vsd_addressline1 = t.providerAddress?.line1,
                //    vsd_addressline2 = t.providerAddress?.line2,
                //    vsd_city = t.providerAddress?.city,
                //    vsd_province = t.providerAddress?.province,
                //    vsd_country = t.providerAddress?.country,
                //    vsd_postalcode = t.providerAddress?.postalCode,

                //    // TODO: It looks like we're using this object in two different places - confirm that we can safely ignore the following fields in this context
                //    vsd_firstname = "", // TODO: We don't collect a split name here
                //    vsd_middlename = "", // TODO: We don't collect a split name here
                //    vsd_lastname = "", // TODO: We don't collect a split name here
                //    vsd_alternatephonenumber = "", // TODO: We don't collect an alternate phone number
                //    vsd_email = "", // TODO: We don't collect an email here
                //    //vsd_preferredmethodofcontact = 1, // TODO: We don't collect a contact method here
                //    //vsd_preferredmethodofcontact = model.RepresentativeInformation.representativePreferredMethodOfContact, // TODO: This isn't correct either
                //    vsd_relationship1 = "", // TODO: We don't collect a relationship here

                //}).ToArray();
            }

            if (model.ExpenseInformation != null)
            {
                // what are we doing here? some kind of CSV string for all of the selected expenses/benefits? seems bonkers...
                //application.Application.vsd_cvap_benefitsrequested = model.ExpenseInformation.haveCrimeSceneCleaningExpenses.ToString();
                application.Application.vsd_cvap_benefitsrequestedother = model.ExpenseInformation.otherSpecificExpenses;

                //application.Application.vsd_cvap_otherbenefits = model.ExpenseInformation.haveDisabilityPlanBenefits.ToString();
                application.Application.vsd_cvap_otherbenefitsother = model.ExpenseInformation.otherSpecificBenefits;
            }

            if (model.EmploymentIncomeInformation != null)
            {
                // what is with all the "ifm" stuff?
                if (model.EmploymentIncomeInformation.wereYouEmployedAtTimeOfCrime.HasValue)
                {
                    application.Application.vsd_cvap_ifmemployedduringcrime = model.EmploymentIncomeInformation.wereYouEmployedAtTimeOfCrime;
                }
                application.Application.vsd_cvap_ifmatworkduringcrime = model.EmploymentIncomeInformation.wereYouAtWorkAtTimeOfIncident;
                application.Application.vsd_cvap_ifmwcbclaimnumber = model.EmploymentIncomeInformation.workersCompensationClaimNumber;
                application.Application.vsd_cvap_ifmmissedwork = model.EmploymentIncomeInformation.didYouMissWorkDueToCrime;
                if (model.EmploymentIncomeInformation.daysWorkMissedStart.HasValue)
                {
                    application.Application.vsd_cvap_ifmmissedworkstart = model.EmploymentIncomeInformation.daysWorkMissedStart;
                }
                if (model.EmploymentIncomeInformation.daysWorkMissedEnd.HasValue)
                {
                    application.Application.vsd_cvap_ifmmissedworkend = model.EmploymentIncomeInformation.daysWorkMissedEnd;
                }
                application.Application.vsd_cvap_ifmlostwages = model.EmploymentIncomeInformation.didYouLoseWages;
                if (model.EmploymentIncomeInformation.areYouSelfEmployed > 0)
                {
                    application.Application.vsd_cvap_ifmselfemployed = model.EmploymentIncomeInformation.areYouSelfEmployed;
                }
                application.Application.vsd_cvap_ifmcontactemployer = model.EmploymentIncomeInformation.mayContactEmployer; 
            }

            if (model.RepresentativeInformation != null)
            {
                application.Application.vsd_cvap_onbehalfofdeclaration = model.RepresentativeInformation.completingOnBehalfOf;
            }

            if (model.DeclarationInformation!= null)
            {
                // TODO: Apparently we don't do anything with this information? No obvious fields on the Dynamics side to feed this into
            }

            if (model.AuthorizationInformation != null)
            {
                application.Application.vsd_cvap_onbehalfofdeclaration = model.RepresentativeInformation.completingOnBehalfOf;
                application.Application.vsd_authorizationsignature = model.AuthorizationInformation.signature;
            }

            application.Application.vsd_applicantssignature = model.AuthorizationInformation.signature; // TODO: where does this come from?
            //application.Application.vsd_cvap_optionalauthorization = 0; // TODO: where does this come from?
            //application.Application.vsd_optionalauthorizationsignature = ""; // TODO: where does this come from?

            //application.DocumentCollection = new Documentcollection[1]; // TODO: bind collection
            

            return application;
        }

        public static ApplicationDynamicsModel GetApplicationDefaults()
        {
            return new ApplicationDynamicsModel
            {
                Application = new Application
                {
                    vsd_applicanttype = 100000002,
                    //VsdApplicantsfirstname = "CVAP DEV",
                    //VsdApplicantslastname = "Form Test",
                    //VsdApplicantsbirthdate = "1982-05-05T00:00:00",
                    //VsdApplicantsbirthdate = new DateTime(1983, 6, 4), //"1982-05-05T00:00:00",
                    //VsdApplicantsgendercode = 100000000,
                    //VsdApplicantsmaritalstatus = 100000000,
                    //VsdCvapTypeofcrime = "Break-in",
                    vsd_applicantsemail = "test@test.com",
                    vsd_applicantsprimaryphonenumber = "250-444-5656",
                    vsd_applicantssignature = "Crime Victim Guy",

                    vsd_cvap_crimestartdate = new DateTime(2018, 6, 14), //"2018-06-03T00:00:00",

                    // TODO: Don't know where these two fields went...
                    // vsd_cvap_authorizationsigneddate = "2019-02-07T00:00:00",
                    // vsd_cvap_declarationsigneddate = "2019-02-07T00:00:00",
                    vsd_cvap_onbehalfofdeclaration = 100000000,
                },
                CourtInfoCollection = new List<Courtinfocollection>
                {
                    new Courtinfocollection
                    {
                        vsd_courtfilenumber = "1234567",
                        vsd_courtlocation = "Victoria"
                    }
                }.ToArray(),
                //ProviderCollection = new List<Providercollection>
                //{
                //    new Providercollection
                //    {
                //        vsd_name = "Mr. Smith",
                //        // TODO: Don't know where this field went
                //        // VsdType = 100000000
                //    }
                //}.ToArray()
            };
        }
    }
}
