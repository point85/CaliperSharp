using Microsoft.VisualStudio.TestTools.UnitTesting;
using Point85.Caliper.UnitOfMeasure;

namespace CaliperSharpTests
{
	[TestClass]
	public class TestClassification : BaseTest
	{
		[TestMethod]
		public void TestClassifications()
		{
			UnitType ut;

			UnitOfMeasure one = sys.GetOne();
			UnitOfMeasure s = sys.GetSecond();
			UnitOfMeasure m = sys.GetUOM(Unit.METRE);
			UnitOfMeasure kg = sys.GetUOM(Unit.KILOGRAM);
			UnitOfMeasure degC = sys.GetUOM(Unit.CELSIUS);
			UnitOfMeasure amp = sys.GetUOM(Unit.AMPERE);
			UnitOfMeasure mol = sys.GetUOM(Unit.MOLE);
			UnitOfMeasure cd = sys.GetUOM(Unit.CANDELA);


			// base types
			Assert.IsTrue(one.Classify().UOMType.Equals(UnitType.UNITY));
			Assert.IsTrue(s.Classify().UOMType.Equals(UnitType.TIME));
			Assert.IsTrue(m.Classify().UOMType.Equals(UnitType.LENGTH));
			Assert.IsTrue(kg.Classify().UOMType.Equals(UnitType.MASS));
			Assert.IsTrue(degC.Classify().UOMType.Equals(UnitType.TEMPERATURE));
			Assert.IsTrue(amp.Classify().UOMType.Equals(UnitType.ELECTRIC_CURRENT));
			Assert.IsTrue(mol.Classify().UOMType.Equals(UnitType.SUBSTANCE_AMOUNT));
			Assert.IsTrue(cd.Classify().UOMType.Equals(UnitType.LUMINOSITY));
			Assert.IsTrue(sys.GetUOM(Unit.US_DOLLAR).Classify().UOMType.Equals(UnitType.CURRENCY));
			Assert.IsTrue(sys.GetUOM(Unit.BIT).Classify().UOMType.Equals(UnitType.COMPUTER_SCIENCE));

			// area
			UnitOfMeasure uom = sys.GetUOM(Unit.FOOT).Power(2);
			Assert.IsTrue(uom.Classify().UOMType.Equals(UnitType.AREA));

			// volume
			Assert.IsTrue(m.Multiply(m).Multiply(m).Classify().UOMType.Equals(UnitType.VOLUME));

			// density
			Assert.IsTrue(kg.Divide(m.Power(3)).Classify().UOMType.Equals(UnitType.DENSITY));

			// speed
			Assert.IsTrue(m.Divide(s).Classify().UOMType.Equals(UnitType.VELOCITY));

			// volumetric flow
			Assert.IsTrue(m.Power(3).Divide(s).Classify().UOMType.Equals(UnitType.VOLUMETRIC_FLOW));

			// mass flow
			Assert.IsTrue(kg.Divide(s).Classify().UOMType.Equals(UnitType.MASS_FLOW));

			// frequency
			Assert.IsTrue(one.Divide(s).Classify().UOMType.Equals(UnitType.FREQUENCY));

			// acceleration
			Assert.IsTrue(m.Divide(s.Power(2)).Classify().UOMType.Equals(UnitType.ACCELERATION));

			// force
			Assert.IsTrue(m.Multiply(kg).Divide(s.Power(2)).Classify().UOMType.Equals(UnitType.FORCE));

			// pressure
			Assert.IsTrue(kg.Divide(m).Divide(s.Power(2)).Classify().UOMType.Equals(UnitType.PRESSURE));

			// energy
			Assert.IsTrue(kg.Multiply(m).Multiply(m).Divide(s.Power(2)).Classify().UOMType.Equals(UnitType.ENERGY));

			// Power
			Assert.IsTrue(kg.Multiply(m).Multiply(m).Divide(s.Power(3)).Classify().UOMType.Equals(UnitType.POWER));

			// electric charge
			Assert.IsTrue(s.Multiply(amp).Classify().UOMType.Equals(UnitType.ELECTRIC_CHARGE));

			// electromotive force
			Assert.IsTrue(kg.Multiply(m.Power(2)).Divide(amp).Divide(s.Power(3)).Classify().UOMType.Equals(UnitType.ELECTROMOTIVE_FORCE));

			// electric resistance
			Assert.IsTrue(kg.Multiply(m.Power(-3)).Multiply(amp.Power(2)).Multiply(s.Power(4)).Classify().UOMType.Equals(UnitType.ELECTRIC_RESISTANCE));

			// electric capacitance
			Assert.IsTrue(s.Power(-3).Multiply(amp.Power(-2)).Multiply(m.Power(2)).Divide(kg).Classify().UOMType.Equals(UnitType.ELECTRIC_CAPACITANCE));

			// electric permittivity				
			Assert.IsTrue(s.Power(4).Multiply(amp.Power(2)).Multiply(m.Power(-3)).Divide(kg).Classify().UOMType.Equals(UnitType.ELECTRIC_PERMITTIVITY));

			// electric field strength
			Assert.IsTrue(amp.Divide(m).Classify().UOMType.Equals(UnitType.ELECTRIC_FIELD_STRENGTH));

			// magnetic flux
			Assert.IsTrue(kg.Divide(amp).Divide(s.Power(2)).Multiply(m.Power(2)).Classify().UOMType.Equals(UnitType.MAGNETIC_FLUX));

			// magnetic flux density
			Assert.IsTrue(kg.Divide(amp).Divide(s.Power(2)).Classify().UOMType.Equals(UnitType.MAGNETIC_FLUX_DENSITY));

			// inductance
			Assert.IsTrue(kg.Multiply(amp.Power(-2)).Divide(s.Power(2)).Multiply(m.Power(2)).Classify().UOMType.Equals(UnitType.ELECTRIC_INDUCTANCE));

			// conductance
			Assert.IsTrue(kg.Power(-1).Multiply(amp.Power(2)).Multiply(s.Power(3)).Multiply(m.Power(-2)).Classify().UOMType.Equals(UnitType.ELECTRIC_CONDUCTANCE));

			// luminous flux
			ut = cd.Multiply(one).Classify().UOMType;
			Assert.IsTrue(ut.Equals(UnitType.LUMINOUS_FLUX) || ut.Equals(UnitType.LUMINOSITY));

			// illuminance
			Assert.IsTrue(cd.Divide(m.Power(2)).Classify().UOMType.Equals(UnitType.ILLUMINANCE));

			// radiation dose absorbed and effective
			ut = m.Power(2).Divide(s.Power(2)).Classify().UOMType;
			Assert.IsTrue(ut.Equals(UnitType.RADIATION_DOSE_ABSORBED) || ut.Equals(UnitType.RADIATION_DOSE_EFFECTIVE));

			// radiation dose rate
			Assert.IsTrue(m.Power(2).Divide(s.Power(3)).Classify().UOMType.Equals(UnitType.RADIATION_DOSE_RATE));

			// radioactivity
			ut = s.Power(-1).Classify().UOMType;
			Assert.IsTrue(ut.Equals(UnitType.RADIOACTIVITY) || ut.Equals(UnitType.FREQUENCY));

			// catalytic activity
			Assert.IsTrue(mol.Divide(s).Classify().UOMType.Equals(UnitType.CATALYTIC_ACTIVITY));

			// dynamic viscosity
			Assert.IsTrue(kg.Divide(s).Multiply(m).Classify().UOMType.Equals(UnitType.DYNAMIC_VISCOSITY));

			// kinematic viscosity
			Assert.IsTrue(m.Power(2).Divide(s).Classify().UOMType.Equals(UnitType.KINEMATIC_VISCOSITY));

			// reciprocal length
			Assert.IsTrue(one.Divide(m).Classify().UOMType.Equals(UnitType.RECIPROCAL_LENGTH));

			// plane angle
			Assert.IsTrue(sys.GetTypeMap(UnitType.PLANE_ANGLE).IsEmpty);
			Assert.IsTrue(sys.GetUOM(Unit.RADIAN).GetBaseUnitsOfMeasure().Count == 0);

			// solid angle
			Assert.IsTrue(sys.GetTypeMap(UnitType.SOLID_ANGLE).IsEmpty);
			Assert.IsTrue(sys.GetUOM(Unit.STERADIAN).GetBaseUnitsOfMeasure().Count == 0);

			// time squared
			Assert.IsTrue(s.Power(2).Classify().UOMType.Equals(UnitType.TIME_SQUARED));

			// molar concentration
			Assert.IsTrue(mol.Divide(m.Power(3)).Classify().UOMType.Equals(UnitType.MOLAR_CONCENTRATION));

			// irradiance
			Assert.IsTrue(kg.Divide(s.Power(3)).Classify().UOMType.Equals(UnitType.IRRADIANCE));
		}
	}
}
